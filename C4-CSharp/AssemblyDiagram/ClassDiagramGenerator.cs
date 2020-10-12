using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace C4_CSharp.AssemblyDiagram
{
    public class ClassDiagramGenerator
    {
        private HashSet<string> types = new HashSet<string>();
        private Accessibilities _ignoreMemberAccessibilities;
        private RelationshipCollection _relationships
            = new RelationshipCollection();
        private TextWriter writer;
        private readonly string indent;
        private int nestingDepth = 0;
        private bool _createAssociation;

        public ClassDiagramGenerator(TextWriter writer, string indent, Accessibilities ignoreMemberAccessibilities = Accessibilities.None, bool createAssociation = true)
        {
            this.writer = writer;
            this.indent = indent;
            _ignoreMemberAccessibilities = ignoreMemberAccessibilities;
            _createAssociation = createAssociation;
        }

        public void Generate(Assembly assembly)
        {
            List<Type> types = assembly.GetTypes().ToList<Type>();
            foreach (Type type in types)
            {
                typesInAssembly.Add(type.Name);
            }

            WriteLine("@startuml");
            WriteLine("left to right direction");
            GenerateInternal(assembly);
            BuildRelations(assembly);
            WriteLine("@enduml");
        }

        public void GenerateInternal(Assembly assembly)
        {
            List<Type> types = assembly.GetTypes().ToList<Type>();

            foreach (Type type in types)
            {
                    AddTypeDeclaration(type);
            }
        }


        //public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        //{
        //    VisitTypeDeclaration(node, () => base.VisitInterfaceDeclaration(node));
        //}

        //public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        //{
        //    VisitTypeDeclaration(node, () => base.VisitClassDeclaration(node));
        //}

        //public override void VisitStructDeclaration(StructDeclarationSyntax node)
        //{
        //    if (SkipInnerTypeDeclaration(node)) { return; }

        //    _relationships.AddInnerclassRelationFrom(node);
        //    _relationships.AddInheritanceFrom(node);

        //    var typeName = TypeNameText.From(node);
        //    var name = typeName.Identifier;
        //    var typeParam = typeName.TypeArguments;
        //    var type = $"{name}{typeParam}";

        //    types.Add(name);

        //    WriteLine($"class {type} <<struct>> {{");

        //    nestingDepth++;
        //    base.VisitStructDeclaration(node);
        //    nestingDepth--;

        //    WriteLine("}");
        //}

        //public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        //{
        //    if (SkipInnerTypeDeclaration(node)) { return; }

        //    _relationships.AddInnerclassRelationFrom(node);

        //    var type = $"{node.Identifier}";

        //    types.Add(type);

        //    WriteLine($"{node.EnumKeyword} {type} {{");

        //    nestingDepth++;
        //    base.VisitEnumDeclaration(node);
        //    nestingDepth--;

        //    WriteLine("}");
        //}

        public void HandleConstructor(ConstructorInfo ci)
        {
            //if (IsIgnoreMember(node.Modifiers)) { return; }

            if (!ci.IsPublic && !ci.IsPrivate)
            {
                return;
            }

            var modifiers = GetMemberModifiersText(ci);

            var name = ci.Name;
            if (name == ".ctor")
            {
                name = ci.ReflectedType.Name;
            }

            var args = ci.GetParameters().Select(p => $"{p.Name}:{FixTypeName(p.ParameterType.Name)})");

            WriteLine($"{modifiers} {name}({string.Join(", ", args)})");
        }

        private string FixTypeName(string inputName)
        {
            string outputName = inputName
                .Replace("System.String", "string")
                .Replace("System.Boolean", "bool")
                .Replace("String", "string")
                .Replace("Boolean", "bool")
                ;

            return outputName;
        }

        public void HandleMethod(MethodInfo methodInfo)
        {
            //    if (IsIgnoreMember(node.Modifiers)) { return; }

            if (methodInfo.IsSpecialName)
            {
                return;
            }

            var modifiers = GetMemberModifiersText(methodInfo);

            var name = methodInfo.Name;

            foreach (CustomAttributeData attr in methodInfo.CustomAttributes)
            {
                if (attr.AttributeType == typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute))
                {
                    return;
                }
            }

            var returnType = methodInfo.ReturnType;

            var args = methodInfo.GetParameters().Select(p => $"{p.Name}:{FixTypeName(p.ParameterType.Name)}");

            WriteLine($"{modifiers} {name}({string.Join(", ", args)}) : {FixTypeName(returnType.Name)}");
        }

        public void HandleProperty(PropertyInfo propertyInfo)
        {
            var type = FixTypeName(propertyInfo.PropertyType.Name);

                var modifiers = GetMemberModifiersText(propertyInfo);
            var name = propertyInfo.Name;

            string accessorString = "";
            if (propertyInfo.CanRead || propertyInfo.CanWrite)
            {
                if (propertyInfo.CanRead)
                {
                    accessorString = "<<get>>";
                }
                if (propertyInfo.CanWrite)
                {
                    if (accessorString.Length > 0)
                    {
                        accessorString = accessorString + " ";
                    }
                    accessorString = accessorString + "<<set>>";
                }
            }

            object initValue = GetDefaultValueForProperty(propertyInfo);

            if (initValue != null)
            {
                WriteLine($"{modifiers} {name} : {FixTypeName(type.ToString())} {accessorString} = {FixValue(initValue)}");
            }
            else
            {
                WriteLine($"{modifiers} {name} : {FixTypeName(type.ToString())} {accessorString}{initValue}");
            }

            CheckForRelation(propertyInfo.PropertyType.Name, propertyInfo.DeclaringType.Name);
        }

        public void HandleField(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsSpecialName)
            {
                return;
            }

            var type = FixTypeName(fieldInfo.FieldType.Name);

            if (fieldInfo.FieldType.IsGenericType)
            {
                string realName = fieldInfo.FieldType.Name.Substring(0, fieldInfo.FieldType.Name.IndexOf('`'));
                string args = "";
                foreach (Type argType in fieldInfo.FieldType.GetGenericArguments())
                {
                    if (args.Length > 0)
                    {
                        args = args + ", ";
                    }
                    args = args + FixTypeName(argType.Name);
                }
                type = $"{realName}<{args}>";
            }

            var modifiers = GetMemberModifiersText(fieldInfo);
            var name = fieldInfo.Name;

            foreach (CustomAttributeData attr in fieldInfo.CustomAttributes)
            {
                if (attr.AttributeType == typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute))
                {
                    return;
                }
            }

            if (fieldInfo.IsLiteral)
            {
                object defVal = fieldInfo.GetRawConstantValue();

                string value = defVal.ToString();
                if (defVal.GetType() == typeof(Int32))
                {
                    value = "0x" + ((int)defVal).ToString("X4");
                }
                WriteLine($"{modifiers} {name} = {value}");
            }
            else
            {
                WriteLine($"{modifiers} {name} : {FixTypeName(type.ToString())}");
                CheckForRelation(fieldInfo.FieldType.Name, fieldInfo.DeclaringType.Name);
            }
        }

        public string FixValue(object initialValue)
        {
            string rtnVal = $"{initialValue}";

            if (initialValue.GetType() == typeof(bool))
            {
                if ((bool)initialValue)
                {
                    rtnVal = "true";
                }
                else
                {
                    rtnVal = "false";
                }
            }

            return rtnVal;
        }

        // https://stackoverflow.com/questions/407337/net-get-default-value-for-a-reflected-propertyinfo
        private static object GetDefaultValueForProperty(PropertyInfo property)
        {
            var defaultAttr = property.GetCustomAttribute(typeof(DefaultValueAttribute));
            if (defaultAttr != null)
                return " = " +(defaultAttr as DefaultValueAttribute).Value;

            var propertyType = property.PropertyType;
            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        private static object GetDefaultValueForProperty(FieldInfo field)
        {
            var defaultAttr = field.GetCustomAttribute(typeof(DefaultValueAttribute));
            if (defaultAttr != null)
                return " = " + (defaultAttr as DefaultValueAttribute).Value;

            var propertyType = field.FieldType;
            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        private void WriteLine(string line)
        {
            var space = string.Concat(Enumerable.Repeat(indent, nestingDepth));
            writer.WriteLine(space + line);
        }

        private void AddTypeDeclaration(Type inputType)
        {
            // deal with this later
            if (inputType.MemberType == MemberTypes.NestedType)
            {
                return;
            }

            var modifiers = "";
            string keyword = "";

            if (inputType.IsClass)
            {
                keyword = "class";
            }
            else if(inputType.IsEnum)
            {
                keyword = "enum";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("figure this one out");
            }

            if(inputType.Name.StartsWith("<"))
            {
                System.Diagnostics.Debug.WriteLine("figure this one out");
            }

            if (inputType.IsAbstract)
            {
                keyword = "abstract " + keyword;
            }


            var typeName = inputType.Name;
            var name = typeName;
            var typeParam = "";
            var type = $"{name}{typeParam}";

            types.Add(name);

            WriteLine($"{keyword} {type} {modifiers}{{");

            nestingDepth++;
            HandleTypeBody(inputType);
            nestingDepth--;

            WriteLine("}");
        }

        private void HandleTypeBody(Type inputType)
        {
            if (inputType.IsEnum)
            {
                FieldInfo[] fields = inputType.GetFields((BindingFlags)1048575);
                foreach (FieldInfo field in fields)
                {
                    HandleField(field);
                }
            }
            else
            {
                PropertyInfo[] properties = inputType.GetProperties((BindingFlags)1048575);
                foreach (PropertyInfo property in properties)
                {
                    HandleProperty(property);
                }

                FieldInfo[] fields = inputType.GetFields((BindingFlags)1048575);
                foreach (FieldInfo field in fields)
                {
                    HandleField(field);
                }


                //HandleConstructors
                ConstructorInfo[] constructors = inputType.GetConstructors((BindingFlags)1048575);
                foreach (ConstructorInfo constructorInfo in constructors)
                {
                    HandleConstructor(constructorInfo);
                }

                //Handle Methods
                MethodInfo[] methods = inputType.GetMethods((BindingFlags)1048575);
                foreach (MethodInfo methodInfo in methods)
                {
                    HandleMethod(methodInfo);
                }
            }
            CheckForInheritance(inputType.BaseType.Name, inputType.Name);
        }

        private string GetMemberModifiersText(MethodBase methodInfo)
        {
            string rtnVal = "";

            if (methodInfo.IsPublic)
            {
                rtnVal = "+";
                if (methodInfo.IsAbstract)
                {
                    rtnVal = rtnVal + " {abstract}";
                }
            }
            else if (methodInfo.IsPrivate)
            {
                rtnVal = "-";
            }
            //else if (methodInfo.IsProtected)
            //{
            //    return "#";
            //}
            else if (methodInfo.IsAbstract)
            {
                rtnVal = "$";
            }
            else if (methodInfo.IsStatic)
            {
                rtnVal = "$";
            }

            if (!methodInfo.IsConstructor)
            {
                if (IsOverride((MethodInfo)methodInfo))
                {
                    rtnVal = rtnVal + " <<override>>";
                }
            }

            return rtnVal;
        }

        private string GetMemberModifiersText(FieldInfo fieldInfo)
        {
            string rtnVal = "";

            if (fieldInfo.IsPublic)
            {
                rtnVal = "+";
            }
            else if (fieldInfo.IsPrivate)
            {
                rtnVal = "-";
            }
            else if (fieldInfo.IsStatic)
            {
                rtnVal = "$";
            }

            return rtnVal;
        }


        // https://stackoverflow.com/questions/2932421/detect-if-a-method-was-overridden-using-reflection-c/10020948
        public bool IsOverride(MethodInfo methodInfo)
        {
            return (methodInfo.GetBaseDefinition() != methodInfo);
        }

        private string GetMemberModifiersText(PropertyInfo methodInfo)
        {
            // assume public for now
                return "+";
        }

        List<string> typesInAssembly = new List<string>();
        List<string> relations = new List<string>();

        private void CheckForRelation(string typeName, string className)
        {
            if (typesInAssembly.Contains(typeName))
            {
                if (!relations.Contains($"{className} o-- {typeName}"))
                {
                    relations.Add($"{className} o-- {typeName}");
                }
            }
        }

        private void CheckForInheritance(string typeName, string className)
        {
            if (typesInAssembly.Contains(typeName))
            {
                if (!relations.Contains($"{typeName} --|> {className}"))
                {
                    relations.Add($"{typeName} --|> {className}");
                }
            }
        }

        private void BuildRelations(Assembly assembly)
        {
            foreach (string relation in relations)
            {
                WriteLine(relation);
            }
        }
    }
}

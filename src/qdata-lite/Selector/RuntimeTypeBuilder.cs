using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace RoyLab.QData.Lite.Selector
{
    internal static class RuntimeTypeBuilder
    {
        private static readonly AssemblyName assemblyName = new AssemblyName {Name = "DynamicLinqTypes"};
        private static readonly ModuleBuilder moduleBuilder;
        private static readonly Dictionary<string, Type> builtTypes = new Dictionary<string, Type>();

        static RuntimeTypeBuilder()
        {
            moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(assemblyName.Name);
        }

        public static Type BuildDynamicType(IEnumerable<PropertyInfo> fields)
        {
            var fieldsToGenerate = fields?.Where(f => f != null).OrderBy(f => f.Name).Distinct().ToList();
            if (fieldsToGenerate == null || fieldsToGenerate.Count == 0)
            {
                return null;
            }

            for (var i = 1; i < fieldsToGenerate.Count; i++)
            {
                if (fieldsToGenerate[i].Name.Equals(fieldsToGenerate[i - 1].Name))
                {
                    return null;
                }
            }

            var className = string.Join(";", fieldsToGenerate.Select(f => $"{f.Name}:{f.PropertyType.Name}"));

            try
            {
                Monitor.Enter(builtTypes);

                if (builtTypes.ContainsKey(className))
                    return builtTypes[className];

                var typeBuilder = moduleBuilder.DefineType(className,
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var propertyInfo in fieldsToGenerate)
                {
                    var fieldBuilder = typeBuilder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType,
                        FieldAttributes.Private);
                    var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault,
                        propertyInfo.PropertyType, null);

                    propertyBuilder.SetGetMethod(CreatePropertyGetter(typeBuilder, fieldBuilder));
                    propertyBuilder.SetSetMethod(CreatePropertySetter(typeBuilder, fieldBuilder));
                }

                builtTypes[className] = typeBuilder.CreateType();

                return builtTypes[className];
            }
            catch
            {
                // ignored
            }
            finally
            {
                Monitor.Exit(builtTypes);
            }

            return null;
        }

        private static MethodBuilder CreatePropertyGetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + fieldBuilder.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                fieldBuilder.FieldType, Type.EmptyTypes);

            var getIl = getMethodBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        private static MethodBuilder CreatePropertySetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + fieldBuilder.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new[] {fieldBuilder.FieldType});

            var setIl = setMethodBuilder.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);
            setIl.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }
    }
}
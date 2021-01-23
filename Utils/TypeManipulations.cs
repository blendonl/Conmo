using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conmo.Models;
using Container = System.ComponentModel.Container;

namespace Conmo.Utils {
    public class TypeManipulations { 
        public static Type? BaseType(Type type) {
            Type? baseType = type?.BaseType ?? null ;
            
            while (baseType?.BaseType != null && !baseType.BaseType.Name.Equals("Object")) {
                baseType = baseType.BaseType;
            }

            return baseType != null && !baseType.Name.Equals("Object") ? baseType : type;
        }
        public static List<Type> GetAllModelTypes(Type type) {
            List<Type> models = new List<Type>();
            
            models.AddRange(type.Assembly.GetTypes().Where(tpe => tpe?.Namespace?.Contains("Model") ?? false).ToList());
            
            foreach (var assembly in GetAllReferencedAssemblies(type)) {
                List<Type> temp = assembly.GetTypes().Where(t => !String.IsNullOrEmpty(t.Namespace) && t.Namespace.Contains("Model")).ToList();
                if (temp != null) {
                    models.AddRange(temp);
                }
            }

            return models;
        }
        public static List<Assembly> GetAllReferencedAssemblies(Type type) {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in type.Assembly.GetReferencedAssemblies()) {
                if (!assembly.FullName.Equals("System.Runtime")) {
                    assemblies.Add(Assembly.Load(assembly));
                }
            }
            return assemblies;
        }
        
        public static List<Type> GetAllReferencedTypes(Type type) {
                List<Type> models = new List<Type>();
    
                foreach (var assembly in GetAllReferencedAssemblies(type))
                    foreach (var tpe in assembly
                         .GetTypes()
                         .Where(t => 
                             t.Namespace != null && 
                             t.Namespace.Contains("Model"))) 
                         models.Add(tpe);
    
                return models;
                 
        }
        
        public static List<Type> GetAllTypesThatExtends(Type type) { 
            List<Type> types = new List<Type>();
            
            List<Type> models = GetAllModelTypes(type); 
            models.AddRange(GetAllReferencedTypes(type));
            foreach (var tpe in models) { 
                if (tpe.BaseType != null && tpe.BaseType.Name.Equals(type.Name)) { 
                    types.Add(tpe);
                }
                
            } 
            return types;
        } 
        
        public static object?[]? GetConstructorParameters(Type methodInfo, object parameters) {
            var constructor = methodInfo.GetConstructors()[0];
            return (parameters == null 
                       ? (methodInfo.IsArray 
                           ? new object?[] {new [] {constructor.GetParameters().Length > 0 }} 
                           : new[] {Activator.CreateInstance(methodInfo)}) 
                       : null);
        }
        public static MethodInfo? GetMethodInfo(Type type, string method, object parameters) { 
            List<Type> parametersType = new List<Type>();
            
            if(parameters != null && parameters.GetType().IsArray) 
                foreach (var variable in (object[])parameters) { 
                    parametersType.Add(variable.GetType()); 
                }
            
            if(parameters != null && !parameters.GetType().IsArray)
                   parametersType.Add(parameters.GetType());
   
            return type.GetMethod(method, parametersType.ToArray()); 
        }
        
        public static List<MethodInfo> GetMethodsInfo(Type type) { 
            return type
                   .GetMethods()
                   .Where(method => method.IsPublic && !method.IsSpecialName)
                   .OrderBy(method =>
                       (!method.DeclaringType?.Name.Equals("Menu") ?? false) &&
                       (method.DeclaringType?.Name.Equals(BaseType(type).Name) ?? false)).ToList(); 
        }
        
        public static  List<ParameterInfo>? GetMethodParametersInfo(Type type, string methodName, object parameters) { 
            return GetMethodInfo(type, methodName, parameters)?.GetParameters().ToList() ?? null; 
        }

        public static List<Type> GetMethodParametersType(Type type, string methodName, object parameters) { 
            return GetMethodParametersInfo(type, methodName, parameters).Select(info => info.ParameterType).ToList();
        }
        
        public static  List<PropertyInfo> GetPropertiesInfos(Type type) {
               return type.GetProperties().Where(prop =>
                   prop.CanWrite &&
                   ((prop.PropertyType.BaseType?.Name.Equals("ValueType") ?? false) || prop.PropertyType.Name.Equals("String")) && 
                   !(prop.Name.StartsWith("_") || prop.PropertyType.Name.Equals("Boolean") || prop.Name.EndsWith("Id"))
               ).OrderBy(pro => !pro.DeclaringType?.Name.Equals(BaseType(type).Name)).ToList();
        }
        public static Type GetType(string name, Type type) { 
            Type rez = type.Assembly.GetTypes().FirstOrDefault(tpe => tpe.ContainsGenericParameters && tpe.Name.Equals(name + "`1") || tpe.Name.Equals(name)); 
            
            if(rez == null){ 
                foreach (var assemblyName in type.Assembly.GetReferencedAssemblies()) { 
                    foreach (var tpe in Assembly.Load(assemblyName).GetTypes().Where(t => t.Namespace != null && t.Namespace.Contains(assemblyName?.Name ?? ""))) { 
                        if ((tpe.ContainsGenericParameters && tpe.Name.Equals(name + "`1")) || 
                            tpe.Name.Equals(name) || (tpe.Name + "s").Equals(name) ||
                            tpe.IsInterface && (tpe.Name + "s").Equals("I" +name)) 
                            return tpe; 
                    }
                }
            }
            return rez;
        }
        
        public static Type GetType(string name ) {
            return typeof(Container).Assembly.GetTypes().FirstOrDefault(service => (service.ContainsGenericParameters) ? service.Name.Equals(name+"`1") : service.IsInterface ? service.Name.Equals("I" +name) : service.Name.Equals(name));
        } 
        public static object?[]? ToObjectArray(object? obj) {
             return (obj != null) ? (obj?.GetType().IsArray ?? false ? (object[]) obj! : new [] {obj}) : null;
        } 
        public static Property ToProperty(object parameters) {
             Type type = parameters.GetType();
 
             var typeName = type.BaseType != null ? type.BaseType : type;
             
             return new Property(typeName.Name, typeName, parameters);
        }


        public static string RemoveInterfaceFromName(string name) {
            return name.StartsWith("I") ? name.Substring(1, name.Length - 1) : name;
        }
    }
}
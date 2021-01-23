using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conmo.Utils;

namespace Conmo.Models {
    public class Dependency {
        public Type TypeOfObject { get; set; }
        public object? ActualObject { get; set; }
        public bool Initialised { get; set; }
        
        public bool Needed { get; set; }

        public Dependency(Type typeOfObject) {
            TypeOfObject = typeOfObject;
        }

        public List<Type>? GetConstructorParams() {
            var constructors = TypeOfObject.GetConstructors();
            return constructors?[0]?.GetParameters().Select(parameter => parameter.ParameterType).ToList() ?? null;
        }

        public object? Initialise() {
            Initialised = true;
            return ActualObject = Activator.CreateInstance(TypeOfObject);
        } 
        public object? Initialise(Type generic) {
            Initialised = true;
            if (TypeOfObject.ContainsGenericParameters) {
                var genericType = TypeOfObject.MakeGenericType(new Type[] {generic});
                return ActualObject = Activator.CreateInstance(genericType);
            }
            else {
                Initialised = true;
                return ActualObject = Activator.CreateInstance(TypeOfObject);
            }
        } 
        public object Initialise(object parameters) {
            
            Initialised = true;
            
            return ActualObject = TypeOfObject.GetConstructors()[0].Invoke(TypeManipulations.ToObjectArray(parameters)); 
            
        }
        
        public object? Initialise(Type generic, object parameters) {
             if (GetConstructorParams()?.Count < 1) { 
                 Initialised = true; 
                 if (TypeOfObject.ContainsGenericParameters) {
                    var genericType = TypeOfObject.MakeGenericType(new Type[] {generic});
                    return ActualObject = Activator.CreateInstance(genericType); 
                 } 
                 Initialised = true; 
                 return ActualObject = Activator.CreateInstance(TypeOfObject);
                 
             }

             return null;
        }
        
        public object? InvokeMethod(string method) {
             return TypeManipulations.GetMethodInfo(TypeOfObject, method, null)?.Invoke(ActualObject, null);
        }
        
        public object? InvokeMethod(string method, object parameters) {
            return TypeManipulations.GetMethodInfo(TypeOfObject, method, parameters)?.Invoke(ActualObject, TypeManipulations.ToObjectArray(parameters)) ?? null;
        }

        public object? InvokeMethod(string method, Type type, object parameters) { 
            if (TypeManipulations.GetMethodInfo(TypeOfObject, method, parameters)?.ContainsGenericParameters ?? false) { 
                var genericMethod = TypeManipulations.GetMethodInfo(TypeOfObject, method, parameters)?.MakeGenericMethod(new[] {type}) ?? null;
                
                return genericMethod?.Invoke((TypeManipulations.GetMethodInfo(TypeOfObject, method, parameters)?.IsStatic ?? false) ? null : ActualObject, TypeManipulations.ToObjectArray(parameters)); 
            }
            
            return TypeManipulations.GetMethodInfo(TypeOfObject, method, parameters)?
                  .Invoke(ActualObject, TypeManipulations.ToObjectArray(parameters));
         }

        public void InitialiseProp(Property property) {
            TypeOfObject
                .GetProperties()
                .FirstOrDefault(pro => 
                        pro.PropertyType.Name.Equals(property.PropertyType.Name) && 
                        pro.Name.Equals(property.PropertyName))
                .SetValue(ActualObject, property.PropertyValue);
        }

        public List<Property> GetProperties() {
            List<Property> properties = new List<Property>();

            TypeManipulations.GetPropertiesInfos(TypeOfObject).ForEach(propertyInfo => properties.Add(new Property() {
                PropertyName = propertyInfo.Name, 
                PropertyType = propertyInfo.PropertyType, 
                PropertyValue = propertyInfo?.GetValue(ActualObject) ?? null
            }));
            

            return properties;
        }
        
        private MethodInfo[]? GetViewMethods() {
            if (TypeOfObject != null)
               return TypeOfObject.GetMethods().Where(method => !method.IsAbstract && !method.IsSpecialName && !method.IsPrivate).ToArray();
            return null;

        } 
        public List<string> GetMethodsName() {
              return TypeManipulations.GetMethodsInfo(TypeOfObject)
                  .Where(method =>
                      !method.IsSpecialName &&
                      !method.Name.Equals("Show") &&
                      !method.Name.Equals("Equals") &&
                      !method.Name.Equals("ToString") &&
                      !method.Name.Equals("GetType") &&
                      !method.Name.Equals("GetHashCode"))
                  .Select(method => method.Name)
                  .ToList();
          }
    }
}
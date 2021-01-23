using System;
using System.Linq;
using Conmo.Models;
using Conmo.Utils;

namespace Conmo {
    public abstract class Menu {

        public void Show() {
            CommonViews.Title(StringManipulations.AddSpacesBeetween(GetType().Name));
            
            Dependency dependency = Container.GetDependency(GetType());

            var methodsName = dependency.GetMethodsName();
            
            int userChoice = CommonViews.Menu(StringManipulations.AddSpacesBeetween(methodsName).ToArray());
            
            if (userChoice < methodsName.Count && userChoice >= 0) {
                string methodName = methodsName[userChoice];
                
                if (!methodName.Equals("GoBack")) {
                    CommonViews.Title(StringManipulations.AddSpacesBeetween(methodName));
                    
                    object methodReturnObject = dependency.InvokeMethod(methodName, null);

                    if (methodName.StartsWith("GoTo")) {
                        GoToMethod(methodName, methodReturnObject);
                    }
                    else {

                        InvokeCrudMethod( methodName, methodReturnObject);
                        Console.ReadLine();
                    }
                    Show();
                }
            }
            else {
                Show();
            }

        }

        private void GoToMethod(string methodName, object? parameter) {
            Dependency dep = Container.GetDependency(methodName.Remove(0, 4), GetType());

            string[]? modelNames = null;
            
            try {

                if (parameter != null && (parameter.GetType().IsPrimitive || parameter.GetType().Name.Equals("String"))) {
                    modelNames = dep.GetConstructorParams().Select(item => item.Name).ToArray();
                   
                    parameter = InvokeCrudMethod($"Select{modelNames[0]}", parameter);
                }
                
                dep.Initialise(parameter);
                
                Container.Add(dep);
                
                dep.InvokeMethod("Show", null);
            }
            catch (Exception) {
                Console.WriteLine($"{(modelNames?[0] ?? "")} does not exists");
                Console.ReadLine();
            }


        }
        private object? InvokeCrudMethod(string methodName, object? parameters ) {
            Dependency crudDependency = Container.GetDependency("CrudOperations");
            string method = crudDependency.GetMethodsName().FirstOrDefault(meth => methodName.StartsWith(meth));
            
            if (!String.IsNullOrEmpty(method)) {
                Type modelType = TypeManipulations.GetType(methodName.Remove(0, method.Length), GetType());
                
                if(modelType != null)
                    return crudDependency.InvokeMethod(method, modelType, parameters != null ? method.Equals("Create") ? parameters.GetType().Name.Equals("Property") ? new [] {parameters} : new [] {TypeManipulations.ToProperty(parameters)} : new [] {parameters} : null);
            }

            return null;
        }

        public void GoBack() {
        } 
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Conmo.Utils;

namespace Conmo.Models { 
    public class Container {
        
        private static readonly List<Dependency> Dependencies;
        
        static Container() {
            Dependencies = new List<Dependency>();
        }

        public static Dependency AddDependency(string name, Type type) {
            var tpe = (name != type.Name) ? TypeManipulations.GetType(name, type) : type; 
            
            Dependency dep = new Dependency(tpe);
            
            if (Dependencies.FirstOrDefault(t => t.TypeOfObject != null && t.TypeOfObject.Name.Equals(tpe.Name)) == null) {
                
                if (dep.TypeOfObject.Name.Contains("Services") || (dep.TypeOfObject.BaseType != null && dep.TypeOfObject.BaseType.Name.Equals("Menu") || dep.Needed))
                    Dependencies.Add(dep);
            }

            return dep;
        }

        public static void Add(Dependency dependency) {
            Remove(dependency);
            Dependencies.Add(dependency);
        }

        private static int Find(Dependency dependency) {
            return Dependencies.FindIndex(dep => dep.TypeOfObject.Name.Equals(dependency.TypeOfObject.Name));
        }

        private static bool Remove(Dependency dependency) {
            int index = Find(dependency);

            if (index != -1) {
                Dependencies.RemoveAt(index);
                return true;
            }

            return false;
        }

        private static Dependency InitialiseDependency(string name) {
            
            Dependency dep = AddDependency(name, typeof(Container));
            try {
                dep.Initialise();
            }
            catch (MissingMethodException) {
                return dep;
            }

            return dep;
        }
       
        private static Dependency InitialiseDependency(Type type) { 
            Dependency dep = AddDependency(type.Name, type);
            try {
                dep.Initialise();
            }
            catch (MissingMethodException) {
                return dep;
            }

            return dep;
        } 
        private static Dependency InitialiseDependency(string name, object parameters) {
            Dependency dep = AddDependency(name, typeof(Container));
            dep.Initialise(parameters); 
            
            return dep;
        }
        private static Dependency InitialiseDependency(Type type, object parameters) {
            Dependency dep = AddDependency(type.Name, type);
            dep.Initialise(parameters); 
                    
            return dep;
        }

        private static Dependency InitialiseDependency(string name, Type type) {
            Dependency dep = AddDependency(name, type);
            try {
                dep.Initialise(type);
            }
            catch (MissingMethodException) {
                return dep;
            }

            return dep;
        }
        
        private static Dependency InitialiseDependency(string name, Type type, object parameters) {
            Dependency dep = AddDependency(name, type);
            dep.Initialise(parameters); 
            
            return dep;
        }

        private static Dependency InitialiseDependency(string name, Type type, Type generic) {
            Dependency dep = AddDependency(name, type);        
            dep.Initialise(generic);
            
            return dep;
        }

        private static Dependency InitialiseDependency(string name, Type type, Type generic, object parameters) {
            Dependency dep = AddDependency(name, type);     
            
            dep.Initialise(generic, parameters);
                    
            return dep;
        }
        
        public static Dependency GetDependency(Type type ) { 
            Dependency dependency = FindDependency(type.Name);
           
           if (dependency != null) {
               return dependency;
           }

            return InitialiseDependency(type);
           
        }

        public static Dependency FindDependency(string name) {
             var dependency = Dependencies.FirstOrDefault(dep => dep.TypeOfObject.Name.Equals(name));
            
            if (dependency != null) {
                if (dependency.Initialised) {
                    return dependency;
                }
            }

            return null;
        }
        
        public static Dependency GetDependency(string name) {

            Dependency dependency = FindDependency(name);

            if (dependency != null) {
                return dependency;
            }
            
            return InitialiseDependency(name);
        }
        
        public static Dependency GetDependency(string name, Type type) {

            var dependency = FindDependency(name);

            if (dependency != null) {
                return dependency;
            }
             
            return InitialiseDependency(name, type);
        }

        public static Dependency GetDependency(string name, object paramters) {
            var dependency = FindDependency(name);

            if (dependency != null) {
                return dependency;
            }
                     
            return InitialiseDependency(name, paramters);
        }
        
        public static Dependency GetDependency(Type type, object paramters) {
            var dependency = FindDependency(type.Name);
           
           if (dependency != null) {
               return dependency;
           }
                             
            return InitialiseDependency(type, paramters);
        }
        public static Dependency GetDependency(string name, Type type, Type generic) {
            var dependency = FindDependency(name);
           
           if (dependency != null) {
               return dependency;
           }
            return InitialiseDependency(name, type, generic);
            
        }

        public static Dependency GetDependency(string name, Type type, object parameters) {

              var dependency = FindDependency(name);
                        
            if (dependency != null) {
                return dependency;
            }

            return InitialiseDependency(name, type, parameters);
        }
        
        public static Dependency GetDependency(string name, Type type, Type generic, object parameters) {
                            
            var dependency = FindDependency(name);
                                   
               if (dependency != null) {
                   return dependency;
               }
                                     
            return InitialiseDependency(name, type, generic, parameters);
         }

     
    }
}
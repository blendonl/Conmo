using System;

namespace Conmo.Models {
    public class Property {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object? PropertyValue { get; set; }

        public Property() { }

        public Property(string propertyName, Type propertyType, object propertyValue) {
            PropertyName = propertyName;
            PropertyType = propertyType;
            PropertyValue = propertyValue;
        }
    }
}
namespace Autodesk.Revit.DB
{
    /// <summary>
    /// Minimal stand-in for the SpecTypeId class used to identify parameter data types.
    /// Only the properties required by tests are included.
    /// </summary>
    public static class SpecTypeId
    {
        public static class String
        {
            public static ForgeTypeId Text { get; } = new ForgeTypeId("spec:string:text");
            public static ForgeTypeId Multiline { get; } = new ForgeTypeId("spec:string:multiline");
            public static ForgeTypeId Url { get; } = new ForgeTypeId("spec:string:url");
            public static ForgeTypeId Custom { get; } = new ForgeTypeId("spec:string:custom");
        }

        public static class Int
        {
            public static ForgeTypeId Integer { get; } = new ForgeTypeId("spec:int:integer");
        }

        public static class Boolean
        {
            public static ForgeTypeId YesNo { get; } = new ForgeTypeId("spec:boolean:yesno");
        }

        public static class Reference
        {
            public static ForgeTypeId Material { get; } = new ForgeTypeId("spec:reference:material");
        }

        public static class Number
        {
            public static ForgeTypeId General { get; } = new ForgeTypeId("spec:number");
            public static ForgeTypeId Length { get; } = new ForgeTypeId("spec:length");
            public static ForgeTypeId Area { get; } = new ForgeTypeId("spec:area");
            public static ForgeTypeId Volume { get; } = new ForgeTypeId("spec:volume");
            public static ForgeTypeId Angle { get; } = new ForgeTypeId("spec:angle");
            public static ForgeTypeId HVACDensity { get; } = new ForgeTypeId("spec:hvac:density");
            public static ForgeTypeId HVACPower { get; } = new ForgeTypeId("spec:hvac:power");
            public static ForgeTypeId HVACTemperature { get; } = new ForgeTypeId("spec:hvac:temperature");
            public static ForgeTypeId HVACAirflow { get; } = new ForgeTypeId("spec:hvac:airflow");
            public static ForgeTypeId ElectricalCurrent { get; } = new ForgeTypeId("spec:electrical:current");
            public static ForgeTypeId ElectricalPower { get; } = new ForgeTypeId("spec:electrical:power");
        }
    }
}

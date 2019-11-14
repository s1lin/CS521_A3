public enum SpiceName {
    Tu, Sa, Ca, Ci, Cl, Pe, Su
}

public static class SpiceNames {
    
    public static string ToString(this SpiceName name) {
        switch (name) {
            case SpiceName.Ca:
                return "Ca";
            case SpiceName.Tu:
                return "Tu";
            case SpiceName.Sa:
                return "Sa";
            case SpiceName.Ci:
                return "Ci";
            case SpiceName.Cl:
                return "Cl";
            case SpiceName.Pe:
                return "Pe";
            default:
                return "Su";
        }
    }

    internal static SpiceName Get(string v) {
      
        switch (v) {
            case "Ca":
                return SpiceName.Ca;
            case "Tu":
                return SpiceName.Tu;
            case "Sa":
                return SpiceName.Sa;
            case "Ci":
                return SpiceName.Ci;
            case "Cl":
                return SpiceName.Cl;
            case "Pe":
                return SpiceName.Pe;
            default:
                return SpiceName.Su;
        }
    }
}
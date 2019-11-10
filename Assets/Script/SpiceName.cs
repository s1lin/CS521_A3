using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
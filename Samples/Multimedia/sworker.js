let ImportError = "";
try { importScripts("import.js"); } catch(E) { ImportError = E; }
onconnect = function(e) {
    var port = e.ports[0];
    port.onmessage = function(M) {
        let Num1 = M.data[0];
        let Num2 = M.data[1];
        let Num2th = "th";
        if ((Num2 <= 10) || (Num2 > 20))
        {
            switch (Num2 % 10) {
                case 1: Num2th = "st"; break;
                case 2: Num2th = "nd"; break;
                case 3: Num2th = "rd"; break;
            }
        }
        let NumResult = Num1 + "^" + Num2 + "=" + Math.pow(Num1, Num2) + "; " + Num2 + Num2th + "-root(" + Num1 + ")=";
        if (Num2 != 0) {
            NumResult = NumResult + Math.pow(Num1, 1 / Num2);
        }
        else {
            NumResult = NumResult + "ERROR";
        }
        let Test = "X";
        if (typeof ImpTest !== "undefined") {
            Test = ImpTest();
        }
        if (Test != "works good") {
            NumResult = NumResult + " - script import error: " + ImportError;
        }
        port.postMessage(NumResult);
    };
}


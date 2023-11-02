let ImportError = "";
try { importScripts("import.js"); } catch(E) { ImportError = E; }
this.onmessage = function(M) {
    let Num1 = M.data[0];
    let Num2 = M.data[1];
    let NumResult = Num1 + "*" + Num2 + "=" + (Num1 * Num2) + "; " + Num1 + "/" + Num2 + "=";
    if (Num2 != 0) {
        NumResult = NumResult + (Num1 / Num2);
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
    postMessage(NumResult);
}


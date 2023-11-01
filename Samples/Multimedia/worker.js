this.onmessage = function(M)
{
    let Num1 = M.data[0];
    let Num2 = M.data[1];
    let NumResult = Num1 + "*" + Num2 + "=" + (Num1 * Num2) + "; " + Num1 + "/" + Num2 + "=";
    if (Num2 != 0) {
        NumResult = NumResult + (Num1 / Num2);
    }
    else {
        NumResult = NumResult + "ERROR";
    }
    postMessage(NumResult);
}


function AddSub() {
    let Num1 = parseInt(document.querySelector('[id=\'Num1\']').value);
    let Num2 = parseInt(document.querySelector('[id=\'Num2\']').value);
    let NumResult = Num1 + "+" + Num2 + "=" + (Num1 + Num2) + "; " + Num1 + "-" + Num2 + "=" + (Num1 - Num2);
    document.querySelector('[id=\'NumResult1\']').innerHTML = NumResult;
}


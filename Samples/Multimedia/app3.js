var SWorkerObj = new SharedWorker("sworker.js");

SWorkerObj.port.onmessage = function(M)
{
    SWorkerCallback(M.data);
};

function SWorkerCallback(M)
{
    document.querySelector('[id=\'NumResult3\']').innerHTML = M;
}

function PwrRoot()
{
    let Num1 = parseInt(document.querySelector('[id=\'Num1\']').value);
    let Num2 = parseInt(document.querySelector('[id=\'Num2\']').value);
    SWorkerObj.port.postMessage([ Num1, Num2 ]);
}


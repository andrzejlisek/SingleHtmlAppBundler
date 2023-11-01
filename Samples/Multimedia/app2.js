var WorkerObj = new Worker("worker.js");

WorkerObj.onmessage = function(M)
{
    WorkerCallback(M.data);
};

function WorkerCallback(M)
{
    document.querySelector('[id=\'NumResult2\']').innerHTML = M;
}

function MulDiv()
{
    let Num1 = parseInt(document.querySelector('[id=\'Num1\']').value);
    let Num2 = parseInt(document.querySelector('[id=\'Num2\']').value);
    WorkerObj.postMessage([ Num1, Num2 ]);
}


#include <cstdio>
#include <emscripten.h>
#include <string>
#include <thread>
#include <mutex>

std::mutex Mtx;
std::string Str;

bool LoopMtx = true;

void ThrTestProc()
{
    if (LoopMtx) Mtx.lock();
    for (int I = 0; I < 26; I++)
    {
        if (!LoopMtx) Mtx.lock();
        std::string SX = "X";
        SX[0] = ((char)(I + 65));
        Str = Str + SX;
        if (!LoopMtx) Mtx.unlock();
    }
    if (LoopMtx) Mtx.unlock();
}

void ThrTest()
{
    #ifdef __EMSCRIPTEN_PTHREADS__
        for (int I = 0; I < 10; I++)
        {
            Str = "";
            std::thread Thr1(ThrTestProc);
            std::thread Thr2(ThrTestProc);
            std::thread Thr3(ThrTestProc);
            std::thread Thr4(ThrTestProc);
            std::thread Thr5(ThrTestProc);
            Thr1.join();
            Thr2.join();
            Thr3.join();
            Thr4.join();
            Thr5.join();
            printf("%s\n", Str.c_str());
        }
    #else
        printf("Thread requirements:\n");
        printf("- In compilation: \"-pthread\" parameter compiler parameter and SharedArrayBuffer in web browser.\n");
        printf("- In running: SharedArrayBuffer in web browser.\n");
    #endif
}

int main()
{
    LoopMtx = false;
    printf("Loop mutex disabled - begin\n");
    ThrTest();
    printf("Loop mutex disabled - end\n");
    LoopMtx = true;
    printf("Loop mutex enabled - begin\n");
    ThrTest();
    printf("Loop mutex enabled - end\n");
    return 0;
}


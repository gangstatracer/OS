// Compile using /clr option.
#include <stdlib.h>
using namespace System;
using namespace System::Threading;

int flag=0;
const int buf_size=1024;
int buf[buf_size];
int i=0;


int lock()
	{
	int f=0;
	__asm
	{
		mov ecx,1
		xor eax,eax
		cmpxchg flag, ecx
		jz l1
		jmp l2
		l1:
			mov f,1
		l2:
	}
	if(f==1) return 0; else return 1;
	}
void unlock()
	{
	flag = 0;
	} 

void ThreadProc()
{
	while(lock()){}
    for(int j=0;j<(buf_size>>1);j++)
	{
		buf[i]=1;
		i++;
		Thread::Sleep(rand()%2);
	}
	unlock();
}
void ThreadProc2()
   {
	while(lock()){}
     for(int j=0;j<(buf_size>>1);j++)
	{
		buf[i]=2;
		i++;
		Thread::Sleep(rand()%2);
	}
	unlock();
   }



int main()
{
   Thread^ oThread = gcnew Thread( gcnew ThreadStart( &ThreadProc ) );
   Thread^ oThread2 = gcnew Thread( gcnew ThreadStart( &ThreadProc2 ) );
   oThread->Start();
   oThread2->Start();
   oThread->Join();
   oThread2->Join();
   for(int j = 0 ; j <buf_size ; j++)
	{
		Console::Write(buf[j]);
	}
	return 0;	

   return 0;
}
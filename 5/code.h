#include <stdio.h>
#include <stdlib.h>
#include <ucontext.h>
#include <unistd.h>
#include <signal.h>
#include <string.h>
#include <time.h>
#include <queue>

const int threads_cnt = 10;
ucontext_t main_c, schedule_c, threads_c[threads_cnt];
char schedule_stack[SIGSTKSZ], threads_st[threads_cnt][SIGSTKSZ];
int thread_cur, threads_sleep[threads_cnt];
queue<int> threads;
void kb_thread(void * func)
{
	getcontext(&threads_c[thread_cur]);
	threads_c[thread_cur].uc_link = &schedule_c;
    threads_c[thread_cur].uc_stack.ss_sp = threads_st[thread_cur];
    threads_c[thread_cur].uc_stack.ss_size = SIGSTKSZ;
	makecontext(&threads_c[thread_cur], func, 0);
}

void kb_sleep(int time)
{
	if (time > 0)
	{
		printf("Process %d fell asleep for %d seconds\n", thread_cur, time);
		threads_sleep[thread_cur] = time;
		swapcontext(&threads_c[thread_cur], &schedule_c);
	}
}

void kb_exit()
{
	printf("task %d ends\n", thread_cur);
	threads_sleep[thread_cur] = -1;
}

void kb_func()
{
	while(1)
	{
		printf("my name is %d\n", thread_cur);
		kb_sleep(rand() % 2);
	}
	kb_exit();
}

void AlarmHandler(int SigNumber, siginfo_t * SI, void * icontext)
{
	for(int i=0;i<threads_cnt;i++) threads_sleep[i]--;
	alarm(1);
	if (thread_cur != -1)
		threads_c[thread_cur] = *(ucontext_t*)icontext;
	thread_cur = -1;
	setcontext(&schedule_c);
}

void schedule(int sig)
{
	thread_cur = -1;
	int id = treads.front();
	if(id!= NULL)
	{
		threads.pop();
		threads.push(id);
		printf("wake up task %d\n", id);
		thread_cur = id;
	}
	setcontext(&threads_c[thread_cur]);
}



int main()
{
	struct sigaction main_act;
	//memset(&main_act, 0, sizeof(main_act));
	main_act.sa_flags = SA_SIGINFO;
	main_act.sa_sigaction = AlarmHandler;
	sigaction(SIGALRM, &main_act, NULL);
	getcontext(&schedule_c);
	schedule_c.uc_link = &main_c;
    schedule_c.uc_stack.ss_sp = schedule_stack;
    schedule_c.uc_stack.ss_size = SIGSTKSZ;
	makecontext(&schedule_c,(void (*)(void))schedule, 1, SIGUSR1);
	for(int i=0;i<SIGSTKSZ;i++)threads_sleep[i] = 0;
	for (thread_cur = 0; thread_cur < threads_cnt; thread_cur++)
	{
		kb_thread();
		threads.pop_back(thread_kur);
	}
	alarm(1);
	swapcontext(&main_c, &schedule_c);
}
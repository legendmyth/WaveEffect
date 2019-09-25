/* Replace "dll.h" with the name of your header */
#include "wave.h"
#include<stdio.h>
#include<windows.h>
#include<math.h> 
#include<stdlib.h>
#include<string.h>

DLLIMPORT void HelloWorld()
{
	MessageBox(0,"Hello World from DLL!\n","Hi",MB_ICONINFORMATION);
}
DLLIMPORT void CalcMapTransform(int height,int width, Vector *vector,unsigned  char *arrDst,unsigned char *arrSource)
{
    for (int j = 0; j < height; j++)
    {
        for (int i = 0; i < width; i++)
        {
            int tranx = i + vector[j*width+i].x;
            int trany = j + vector[j*width+i].y;
            if (tranx >= 0 && tranx < width && trany >= 0 && trany < height)
            {
                arrDst[i * 3 + j * width * 3] = arrSource[tranx * 3 + trany * width * 3];
                arrDst[i * 3 + j * width * 3 + 1] = arrSource[tranx * 3 + trany * width * 3 + 1];
                arrDst[i * 3 + j * width * 3 + 2] = arrSource[tranx * 3 + trany * width * 3 + 2];
            }
        }
    }
}

DLLIMPORT void SingleWaveCalc(Wave wave,int width,int height,Vector* vector,int delay)
{
//	char *msg=malloc(200);
//	sprintf(msg,"wave.x:%d,wave.y:%d,wave.p:%d,,wave.amplitude:%f,wave.waveLength:%f,sizeof(wave):%d,sizeof(double):%d,sizeof(vector):%d",wave.x,wave.y,wave.p,wave.amplitude,wave.waveLength,sizeof(Wave),sizeof(double),sizeof(Vector));
//	MessageBox(0,msg,"Hi",MB_ICONINFORMATION);
	//printf(msg);
	//free(msg); 
	double p1 = sqrt((width - wave.x) * (width - wave.x) + (height - wave.y) * (height - wave.y));
    double p2 = sqrt((width - wave.x) * (width - wave.x) + wave.y * wave.y);
    double p3 = sqrt((wave.x * wave.x)+ (height - wave.y) * (height - wave.y));
    double p4 = sqrt((wave.x * wave.x) + (wave.y * wave.y));
    while (wave.p < p1 || wave.p < p2 || wave.p < p3 || wave.p < p4)
    {
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                double p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);
                double min = (wave.p - wave.waveLength)* (wave.p - wave.waveLength);
                double max = (wave.p + wave.waveLength)* (wave.p + wave.waveLength);
                if (p >= min && p <= max)
                {
                    double p0 = sqrt(p);
					vector[j*width + i].x += (int)(wave.amplitude * sin(p0 / wave.waveLength) * (i - wave.x) / p0);
                    vector[j*width + i].y += (int)(wave.amplitude * sin(p0 / wave.waveLength) * (j - wave.y) / p0);                    
                }
                
            }
        }
        
        Sleep(delay);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                double p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);
                double min = (wave.p - wave.waveLength)* (wave.p - wave.waveLength);
                double max = (wave.p + wave.waveLength)* (wave.p + wave.waveLength);
                if (p >= min && p <= max)
                {
                    double p0 = sqrt(p);
					vector[j*width + i].x -= (int)(wave.amplitude * sin(p0 / wave.waveLength) * (i - wave.x) / p0);
                    vector[j*width + i].y -= (int)(wave.amplitude * sin(p0 / wave.waveLength) * (j - wave.y) / p0);                    
                }
                
            }
        }
        wave.p = wave.p + wave.waveLength;
        
    }

}

BOOL WINAPI DllMain(HINSTANCE hinstDLL,DWORD fdwReason,LPVOID lpvReserved)
{
	switch(fdwReason)
	{
		case DLL_PROCESS_ATTACH:
		{
			break;
		}
		case DLL_PROCESS_DETACH:
		{
			break;
		}
		case DLL_THREAD_ATTACH:
		{
			break;
		}
		case DLL_THREAD_DETACH:
		{
			break;
		}
	}
	
	/* Return TRUE on success, FALSE on failure */
	return TRUE;
}

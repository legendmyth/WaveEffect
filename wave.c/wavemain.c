/* Replace "dll.h" with the name of your header */
#include "wave.h"
#include<stdio.h>
#include<windows.h>
#include<math.h>
#include<stdlib.h>
#include<string.h>

DLLIMPORT void HelloWorld() {
	MessageBox(0,"Hello World from DLL!\n","Hi",MB_ICONINFORMATION);
}

DLLIMPORT void InitWaves(Wave *waves,int waveCnt) {
	return;
}

DLLIMPORT void AddWave(Wave *waves,int waveCnt,Wave wave) {
	for(int cnt=0; cnt<waveCnt; cnt++) {
		if(waves[cnt].amplitude == 0.0f) {
			waves[cnt]=wave;
			break;
		}
	}
}

DLLIMPORT void CalcMapTransform(int height,int width, Vector *vector,unsigned  char *arrDst,unsigned char *arrSource) {
	for (int j = 0; j < height; j++) {
		for (int i = 0; i < width; i++) {
			int tranx = i + vector[j*width+i].x;
			int trany = j + vector[j*width+i].y;
			if (tranx >= 0 && tranx < width && trany >= 0 && trany < height) {
				arrDst[i * 3 + j * width * 3] = arrSource[tranx * 3 + trany * width * 3];
				arrDst[i * 3 + j * width * 3 + 1] = arrSource[tranx * 3 + trany * width * 3 + 1];
				arrDst[i * 3 + j * width * 3 + 2] = arrSource[tranx * 3 + trany * width * 3 + 2];
			}
		}
	}
}

DLLIMPORT void SingleWaveCalc(Wave wave,int width,int height,Vector* vector,int delay) {
//	char *msg=malloc(200);
//	sprintf(msg,"wave.x:%d,wave.y:%d,wave.p:%d,,wave.amplitude:%f,wave.waveLength:%f,sizeof(wave):%d,sizeof(float):%d,sizeof(vector):%d",wave.x,wave.y,wave.p,wave.amplitude,wave.waveLength,sizeof(Wave),sizeof(float),sizeof(Vector));
//	MessageBox(0,msg,"Hi",MB_ICONINFORMATION);
	//printf(msg);
	//free(msg);
	float p1 = sqrt((width - wave.x) * (width - wave.x) + (height - wave.y) * (height - wave.y));
	float p2 = sqrt((width - wave.x) * (width - wave.x) + wave.y * wave.y);
	float p3 = sqrt((wave.x * wave.x)+ (height - wave.y) * (height - wave.y));
	float p4 = sqrt((wave.x * wave.x) + (wave.y * wave.y));
	while (wave.p < p1 || wave.p < p2 || wave.p < p3 || wave.p < p4) {
		float min = (wave.p - wave.waveLength)* (wave.p - wave.waveLength);
		float max = (wave.p + wave.waveLength)* (wave.p + wave.waveLength);
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				float p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);
				if (p >= min && p <= max) {
					float p0 = sqrt(p);
					vector[j*width + i].x += (int)(wave.amplitude * sin(p0 / wave.waveLength) * (i - wave.x) / p0);
					vector[j*width + i].y += (int)(wave.amplitude * sin(p0 / wave.waveLength) * (j - wave.y) / p0);
				}

			}
		}

		Sleep(delay);
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				float p = (i - wave.x) * (i - wave.x) + (j - wave.y) * (j - wave.y);

				if (p >= min && p <= max) {
					float p0 = sqrt(p);
					vector[j*width + i].x -= (int)(wave.amplitude * sin(p0 / wave.waveLength) * (i - wave.x) / p0);
					vector[j*width + i].y -= (int)(wave.amplitude * sin(p0 / wave.waveLength) * (j - wave.y) / p0);
				}

			}
		}
		wave.p = wave.p + wave.waveLength;

	}

}

DLLIMPORT void MultiWaveCalc(Wave *waves,int width,int height,Vector* vector,int waveSpeed,int waveCnt,int delay) {
	while(1) {
		for(int cnt=0; cnt<waveCnt; cnt++) {
			if(waves[cnt].amplitude == 0.0f) {
				continue;
			}
			float p1 = sqrt((width - waves[cnt].x) * (width - waves[cnt].x) + (height - waves[cnt].y) * (height - waves[cnt].y));
			float p2 = sqrt((width - waves[cnt].x) * (width - waves[cnt].x) + waves[cnt].y * waves[cnt].y);
			float p3 = sqrt((waves[cnt].x * waves[cnt].x)+ (height - waves[cnt].y) * (height - waves[cnt].y));
			float p4 = sqrt((waves[cnt].x * waves[cnt].x) + (waves[cnt].y * waves[cnt].y));
			if (waves[cnt].p < p1 || waves[cnt].p < p2 || waves[cnt].p < p3 || waves[cnt].p < p4) {
				float min = (waves[cnt].p - waves[cnt].waveLength)* (waves[cnt].p - waves[cnt].waveLength);
				float max = (waves[cnt].p + waves[cnt].waveLength)* (waves[cnt].p + waves[cnt].waveLength);
				for (int j = 0; j < height; j++) {
					for (int i = 0; i < width; i++) {
						float p = (i - waves[cnt].x) * (i - waves[cnt].x) + (j - waves[cnt].y) * (j - waves[cnt].y);
						if (p >= min && p <= max) {
							float p0 = sqrt(p);
							vector[j*width + i].x += (int)(waves[cnt].amplitude * sin(p0 / waves[cnt].waveLength) * (i - waves[cnt].x) / p0);
							vector[j*width + i].y += (int)(waves[cnt].amplitude * sin(p0 / waves[cnt].waveLength) * (j - waves[cnt].y) / p0);
						}

					}
				}
			} else {
				waves[cnt].amplitude=0.0f;
			}
		}
		Sleep(delay);
		for(int cnt=0; cnt<waveCnt; cnt++) {
			if(waves[cnt].amplitude == 0.0f) {
				continue;
			}
			float min = (waves[cnt].p - waves[cnt].waveLength)* (waves[cnt].p - waves[cnt].waveLength);
			float max = (waves[cnt].p + waves[cnt].waveLength)* (waves[cnt].p + waves[cnt].waveLength);
			for (int j = 0; j < height; j++) {
				for (int i = 0; i < width; i++) {
					float p = (i - waves[cnt].x) * (i - waves[cnt].x) + (j - waves[cnt].y) * (j - waves[cnt].y);
					if (p >= min && p <= max) {
						float p0 = sqrt(p);
						vector[j*width + i].x -= (int)(waves[cnt].amplitude * sin(p0 / waves[cnt].waveLength) * (i - waves[cnt].x) / p0);
						vector[j*width + i].y -= (int)(waves[cnt].amplitude * sin(p0 / waves[cnt].waveLength) * (j - waves[cnt].y) / p0);
					}

				}
			}
		}

		for(int cnt=0; cnt<waveCnt; cnt++) {
			if(waves[cnt].amplitude == 0.0f) {
				continue;
			}
			waves[cnt].p = waves[cnt].p + waveSpeed;
		}
		Sleep(delay);
	}


}

BOOL WINAPI DllMain(HINSTANCE hinstDLL,DWORD fdwReason,LPVOID lpvReserved) {
	switch(fdwReason) {
		case DLL_PROCESS_ATTACH: {
			break;
		}
		case DLL_PROCESS_DETACH: {
			break;
		}
		case DLL_THREAD_ATTACH: {
			break;
		}
		case DLL_THREAD_DETACH: {
			break;
		}
	}

	/* Return TRUE on success, FALSE on failure */
	return TRUE;
}


#ifndef _DLL_H_
#define _DLL_H_


#if BUILDING_DLL
#define DLLIMPORT __declspec(dllexport)
#else
#define DLLIMPORT __declspec(dllimport)
#endif

#ifndef PI
#define PI 3.1415926f
#endif

typedef struct{
	int x;
	int y;
}Vector;

typedef struct{
	int x;
    int y;
    int p;
    float waveLength;
    float amplitude;
}Wave;

DLLIMPORT void HelloWorld();

DLLIMPORT void InitWaves(Wave *waves,int waveCnt);

DLLIMPORT void AddWave(Wave *waves,int waveCnt,Wave wave);

DLLIMPORT void CalcMapTransform(int height,int width,Vector *vector,unsigned char *arrDes,unsigned char *arrSource);

DLLIMPORT void SingleWaveCalc(Wave wave,int width,int height,Vector *vector,int delay,int speed);

DLLIMPORT void SingleWaveCalcV2(Wave wave,int width,int height,Vector* vector,int delay,int speed);

DLLIMPORT void MultiWaveCalc(Wave *waves,int width,int height,Vector* vector,int waveSpeed,int waveCnt,int delay);

float calc(float x);

#endif


#include<iostream>
#include<unordered_set>
#include<set>
#include<stack>

using namespace std;

// Clarify pMap values for readability
#define CLEAR 1
#define BLOCKED 0

// helper struct for tracking path data
struct space
{
    bool initialized = false;
    int previous{ 0 };
    double costFromStart{ 0 };
    double costToDest{ 0 };
    double cost{ 0 };
};

// helper typedef to keep path costs associated with prospective path indices
typedef pair<double, int> valueIdxPair;

//
// Utilities for translating to flat array
//
inline int idxFromCoord(int x, int y, int width)
{
    return x + (y * width);
}

inline int xFromIdx(int idx, int width)
{
    return idx % width;
}

inline int yFromIdx(int idx, int width)
{
    return idx / width;
}

// Utility functions for finding legal moves
inline bool canMoveLeft(int idx, int width, const unsigned char* map)
{
    return idx % width > 0
        && *(map + idx - 1) == CLEAR;
}

inline bool canMoveRight(int idx, int width, const unsigned char* map)
{
    return idx % width < (width - 1)
        && *(map + idx + 1) == CLEAR;
}

inline bool canMoveDown(int idx, int width, int height, const unsigned char* map)
{
    return (idx + width) < (width * height)
        && *(map + idx + width) == CLEAR;
}

inline bool canMoveUp(int idx, int width, const unsigned char* map)
{
    return width <= idx &&
        *(map + idx - width) == CLEAR;
}

// A* cost estimator function, moves are limited to compass points, so we only care about absolute difference
inline double getCostToDest(int x, int y, int targetx, int targety)
{
    auto xDiff = abs((double)x - targetx);
    auto yDiff = abs((double)y - targety);
    return xDiff + yDiff;
}

// uses spaceMeasurements array to trace a path back to target from current position. If successful
inline int travelBackAndWritePathToOutBuffer(space spaceMeasurements[], int targetIdx, int width, int height, int* pLength, int* pOutBuffer, int maxBufferLength)
{
    auto travelIdx = targetIdx;
    *pLength = spaceMeasurements[travelIdx].costFromStart;
    if (*pLength <= maxBufferLength)
    {
        int it = *pLength - 1;
        // stack up positions as you trace the route back to the origin of the trace
        while (it >= 0)
        {
            *(pOutBuffer + it) = travelIdx;
            it--;
            travelIdx = spaceMeasurements[travelIdx].previous;
        }
    }

    return *pLength;
}

// Process a node in the search after providing the next step
inline bool completeSearchOrUpdateSpacesWithNewProspect(
    int idx,
    int newIdx,
    int targetIdx,
    int nTargetX,
    int nTargetY,
    int nMapWidth,
    int nMapHeight,
    const unsigned char* pMap,
    int* pathLength,
    int* pOutBuffer,
    int maxBufferLength,
    space* spaceMeasurements,
    unordered_set<int>& stepsTaken,
    valueIdxPair currentSearch,
    set<valueIdxPair>& search)
{
    auto newDistanceFromOrigin = spaceMeasurements[currentSearch.second].costFromStart + 1;
    if (newIdx == targetIdx)
    {
        spaceMeasurements[newIdx].previous = idx;
        spaceMeasurements[newIdx].initialized = true;
        spaceMeasurements[newIdx].costFromStart = newDistanceFromOrigin;
        *pathLength = travelBackAndWritePathToOutBuffer(spaceMeasurements, targetIdx, nMapWidth, nMapHeight, pathLength, pOutBuffer, maxBufferLength);
        return *pathLength != -1;
    }
    else if (stepsTaken.find(newIdx) == stepsTaken.end() && *(pMap + newIdx) == CLEAR)
    {
        auto newGuess = getCostToDest(xFromIdx(newIdx, nMapWidth), yFromIdx(newIdx, nMapWidth), nTargetX, nTargetY);
        auto newCost = newDistanceFromOrigin + newGuess;

        if (spaceMeasurements[newIdx].initialized == false || spaceMeasurements[newIdx].cost >= newCost)
        {
            search.insert(make_pair(newCost, newIdx));
            spaceMeasurements[newIdx].cost = newCost;
            spaceMeasurements[newIdx].costFromStart = newDistanceFromOrigin;
            spaceMeasurements[newIdx].costToDest = newGuess;
            spaceMeasurements[newIdx].previous = idx;
            spaceMeasurements[newIdx].initialized = true;
        }
    }

    return false;
}

int FindPath(const int nStartX, const int nStartY,
    const int nTargetX, const int nTargetY,
    const unsigned char* pMap, const int nMapWidth, const int nMapHeight,
    int* pOutBuffer, const int nOutBufferSize);

int FindPath(const int nStartX, const int nStartY,
    const int nTargetX, const int nTargetY,
    const unsigned char* pMap, const int nMapWidth, const int nMapHeight,
    int* pOutBuffer, const int nOutBufferSize)
{
    if (pMap == nullptr) return -1;

    // avoid work if case where target is destination
    if (nTargetX == nStartX && nTargetY == nStartY)
    {
        return 0;
    }

    if (pOutBuffer != nullptr)
    {
        auto sizeOfpMap = nMapHeight * nMapWidth;

        // Determine starting and target indexes without sorting data into 2d array
        auto startIdx = idxFromCoord(nStartX, nStartY, nMapWidth);
        auto targetIdx = idxFromCoord(nTargetX, nTargetY, nMapWidth);
        unordered_set<int> stepsTaken;
        auto spaceMeasurements{ new space[sizeOfpMap] };
        int x;
        int y;

        // set starting space
        spaceMeasurements[startIdx].cost = 0;
        spaceMeasurements[startIdx].costFromStart = 0;
        spaceMeasurements[startIdx].costToDest = 0;
        spaceMeasurements[startIdx].previous = startIdx;
        spaceMeasurements[startIdx].initialized = true;

        // set up our queue of spaces we'll be checking
        set<valueIdxPair> search;
        search.insert(make_pair(0, startIdx));
        int pathLength{ -1 };
        while (!search.empty())
        {
            // Move to next member of queue
            valueIdxPair currentSearch = *search.begin();
            search.erase(search.begin());
            stepsTaken.insert(currentSearch.second);
            x = xFromIdx(currentSearch.second, nMapWidth);
            y = yFromIdx(currentSearch.second, nMapWidth);
            // Check Up
            if (canMoveUp(currentSearch.second, nMapWidth, pMap) &&
                completeSearchOrUpdateSpacesWithNewProspect(
                    /*Current Coordinates*/ currentSearch.second,
                    /*Next Step*/ idxFromCoord(x, /*Up*/y - 1, nMapWidth),
                    targetIdx, nTargetX, nTargetY,
                    nMapWidth, nMapHeight, pMap, &pathLength, pOutBuffer, nOutBufferSize,
                    spaceMeasurements, stepsTaken, currentSearch, search))
            {
                break;
            }
            // Check Down
            if (canMoveDown(currentSearch.second, nMapWidth, nMapHeight, pMap) &&
                completeSearchOrUpdateSpacesWithNewProspect(
                    /*Current Coordinates*/ currentSearch.second,
                    /*Next Step*/ idxFromCoord(x, /*Down*/y + 1, nMapWidth),
                    targetIdx, nTargetX, nTargetY,
                    nMapWidth, nMapHeight, pMap, &pathLength, pOutBuffer, nOutBufferSize,
                    spaceMeasurements, stepsTaken, currentSearch, search))
            {
                break;
            }
            // Check Left
            if (canMoveLeft(currentSearch.second, nMapWidth, pMap) &&
                completeSearchOrUpdateSpacesWithNewProspect(
                    /*Current Coordinates*/ currentSearch.second,
                    /*Next Step*/ idxFromCoord(/*Left*/x - 1, y, nMapWidth),
                    targetIdx, nTargetX, nTargetY,
                    nMapWidth, nMapHeight, pMap, &pathLength, pOutBuffer, nOutBufferSize,
                    spaceMeasurements, stepsTaken, currentSearch, search))
            {
                break;
            }
            // Check Right
            if (canMoveRight(currentSearch.second, nMapWidth, pMap) &&
                completeSearchOrUpdateSpacesWithNewProspect(
                    /*Current Coordinates*/ currentSearch.second,
                    /*Next Step*/ idxFromCoord(/*Right*/x + 1, y, nMapWidth),
                    targetIdx, nTargetX, nTargetY,
                    nMapWidth, nMapHeight, pMap, &pathLength, pOutBuffer, nOutBufferSize,
                    spaceMeasurements, stepsTaken, currentSearch, search))
            {
                break;
            }
        }

        return pathLength;
    }

    return -1;
}

int main()
{
    unsigned char pMap[] = {
        1, 1, 1, 1,
        0, 1, 0, 1,
        0, 1, 1, 1 };
    int pOutBuffer[12];
    cout << "1 5 9 path: " << FindPath(0, 0, 1, 2, pMap, 4, 3, pOutBuffer, 12) << endl;
    for (int i = 0; i < 12; i++)
    {
        cout << pOutBuffer[i] << ", " << endl;
    }

    cout << endl;

    unsigned char pMap2[] = {
        0, 0, 1,
        0, 1, 1,
        1, 0, 1 };
    int pOutBuffer2[7];
    cout << "unsolvable path: " << FindPath(2, 0, 0, 2, pMap2, 3, 3, pOutBuffer2, 7) << endl;
    for (int i = 0; i < 7; i++)
    {
        cout << pOutBuffer2[i] << ", " << endl;
    }

    unsigned char pMap3[] = {
        1, 1, 1,
        1, 1, 1,
        1, 1, 1 };
    int pOutBuffer3[7];
    cout << "1 0 3 6 path: " << FindPath(2, 0, 0, 2, pMap3, 3, 3, pOutBuffer3, 7) << endl;
    for (int i = 0; i < 7; i++)
    {
        cout << pOutBuffer3[i] << ", " << endl;
    }

    cout << "nullptr path: " << FindPath(0, 0, 0, 0, nullptr, 1, 1, nullptr, -1) << endl;

    unsigned char pMap4[] = {
        1 };
    int pOutBuffer4[1];
    cout << "0 length path: " << FindPath(0, 0, 0, 0, pMap4, 1, 1, pOutBuffer4, 1) << endl;
    for (int i = 0; i < 1; i++)
    {
        cout << pOutBuffer4[i] << ", " << endl;
    }

    unsigned char pMap5[] = {
        1,0,1,1,1,1,1,1,
        1,0,0,0,0,0,0,1,
        1,0,1,1,1,1,1,1,
        1,0,1,0,0,0,0,0,
        1,0,1,1,1,1,1,1,
        1,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1 };
    int pOutBuffer5[42];
    cout << "long circuitous path: " << FindPath(2, 0, 0, 0, pMap5, 8, 7, pOutBuffer5, 5) << endl;
    for (int i = 0; i < 42; i++)
    {
        cout << pOutBuffer5[i] << ", " << endl;
    }

    return 0;
}

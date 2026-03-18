#include <cmath>

//command to export the function to DLL
#define EXPORT extern "C" __declspec(dllexport)

EXPORT double CalculateNewHeading(double targetX, double targetY,
    double missileX, double missileY,
    double currentHeading, double navConstant,
    double previousLosAngle, double* outNewLosAngle)
{
    double dx = targetX - missileX;
    double dy = targetY - missileY;
    double currentLos = std::atan2(dy, dx);

    *outNewLosAngle = currentLos;

    double losRate = currentLos - previousLosAngle;
    const double PI = 3.14159265358979323846;
    while (losRate > PI)  
        losRate -= 2 * PI;
    while (losRate < -PI) 
        losRate += 2 * PI;

    double newHeading = currentHeading + (navConstant * losRate);

    return newHeading;
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionInterpolation
{
    public static class Utility
    {
        public static void QuaternionToEuler()
        {

        }

        public static Quaternion EulerToQuaternion(double heading, double attitude, double bank)
        {
            Quaternion quaternion;
            // Assuming the angles are in radians.
            double c1 = Math.Cos(heading);
            double s1 = Math.Sin(heading);
            double c2 = Math.Cos(attitude);
            double s2 = Math.Sin(attitude);
            double c3 = Math.Cos(bank);
            double s3 = Math.Sin(bank);
            quaternion.W = Math.Sqrt(1.0 + c1 * c2 + c1 * c3 - s1 * s2 * s3 + c2 * c3) / 2.0;
            double w4 = (4.0 * quaternion.W);
            quaternion.X = (c2 * s3 + c1 * s3 + s1 * s2 * c3) / w4;
            quaternion.Y = (s1 * c2 + s1 * c3 + c1 * s2 * s3) / w4;
            quaternion.Z = (-s1 * s3 + c1 * s2 * c3 + s2) / w4;
            return quaternion;
            
        }

        //get euler/quaternion rotation
    }
}

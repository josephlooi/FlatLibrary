using System;
using FlatEngine;
using FlatEngine.Util;
using FlatPhysics.BodyType;

namespace FlatPhysics.Constants
{
    internal static class Collision
    {
        private static FlatVector[] contactList = new FlatVector[2];
        private static double[] magnitudeList = new double[2];
        private static double magnitude;

        internal static bool IsColliding(
            Body bodyA, Body bodyB, 
            out double depth, out FlatVector normal)
        {
            depth = 0;
            normal = FlatVector.Zero;

            if (bodyA is Circle ballA)
            {
                if (bodyB is Circle ballB)
                {
                    return Collision.IsColliding(ballA.Position, ballA.Radius, ballB.Position, ballB.Radius, out depth, out normal);
                }
                else if (bodyB is Polygon polyB)
                {
                    return Collision.IsColliding(polyB.Position, polyB.TransformedVertices, ballA.Position, ballA.Radius, out depth, out normal);
                }
            }
            else if (bodyA is Polygon polyA)
            {
                if (bodyB is Circle ballB)
                {
                    bool result = Collision.IsColliding(polyA.Position, polyA.TransformedVertices, ballB.Position, ballB.Radius, out depth, out normal);
                    normal = -normal;
                    return result;
                }
                else if (bodyB is Polygon polyB)
                {
                    return Collision.IsColliding(polyA.Position, polyA.TransformedVertices, polyB.Position, polyB.TransformedVertices, out depth, out normal);
                }
            }
            return false;
        }
        internal static void Resolve(
            Body bodyA, Body bodyB, double depth, FlatVector normal)
        {
            Collision.ResolveIntersection(bodyA, bodyB, depth * normal);
            if (bodyA.isRotationStatic && bodyB.isRotationStatic)
                Collision.ResolveWithoutRotation(bodyA, bodyB, normal);
            else Collision.ResolveWithRotation(bodyA, bodyB, normal);
        }
        private static int GetContactPoints(
            Body bodyA, Body bodyB, 
            out FlatVector contact1, out FlatVector contact2)
        {
            contact1 = FlatVector.Zero;
            contact2 = FlatVector.Zero;

            if (bodyA is Circle ballA)
            {
                if (bodyB is Circle ballB)
                {
                    contact1 = Collision.GetContactPoint(ballA.Position, ballA.Radius, ballB.Position);
                    return 1;
                }
                else if (bodyB is Polygon polyB)
                {
                    contact1 = Collision.GetContactPoint(ballA.Position, polyB.TransformedVertices);
                    return 1;
                }
            }
            else if (bodyA is Polygon polyA)
            {
                if (bodyB is Circle ballB)
                {
                    contact1 = Collision.GetContactPoint(ballB.Position, polyA.TransformedVertices);
                    return 1;
                }
                else if (bodyB is Polygon polyB)
                {
                    return Collision.GetContactPoints(polyA.TransformedVertices, polyB.TransformedVertices, out contact1, out contact2);
                }
            }
            return 0;
        }
        private static void ResolveIntersection(
            Body bodyA, Body bodyB, FlatVector mtv)
        {
            if (bodyA.isPositionStatic) bodyB.Move(mtv);
            else if (bodyB.isPositionStatic) bodyA.Move(-mtv);
            else
            {
                FlatVector halfMtv = FlatMath.Div2(mtv);
                bodyA.Move(-halfMtv);
                bodyB.Move(halfMtv);
                //if (bodyA.Mass == bodyB.Mass)
                //{
                //    FlatVector halfMtv = FlatMath.Div2(mtv);
                //    bodyA.Move(-halfMtv);
                //    bodyB.Move(halfMtv);
                //}
                //else
                //{
                //    double sumMass = bodyA.Mass + bodyB.Mass;
                //    bodyA.Move(-mtv * bodyB.Mass / sumMass);
                //    bodyB.Move(mtv * bodyA.Mass / sumMass);
                //}
            }
        }



        private static void ResolveWithoutRotation(
            Body bodyA, Body bodyB, FlatVector normal)
        {
            double sumInvMass = bodyA.InvMass + bodyB.InvMass;
            FlatVector relVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

            Collision.ResolveMovementWithoutRotation(
                bodyA, bodyB, normal, relVelocity, sumInvMass, 
                1 + Math.Min(bodyA.Restitution, bodyB.Restitution));

            double frictionStatic = bodyA.FrictionStatic + bodyB.FrictionStatic;
            double frictionDynamic = bodyA.FrictionDynamic + bodyB.FrictionDynamic;
            if (frictionStatic + frictionDynamic == 0) return;

            Collision.ResolveFrictionWithoutRotation(
                    bodyA, bodyB, normal, relVelocity, sumInvMass,
                    FlatMath.Div2(frictionStatic), FlatMath.Div2(frictionDynamic));
        }
        private static void ResolveMovementWithoutRotation(
            Body bodyA, Body bodyB, FlatVector collisionDirection, FlatVector relVelocity,
            double sumInvMass, double restitutionMultiplier)
        {
            double relSpeed = collisionDirection * relVelocity;
            if (relSpeed > 0) return;

            Collision.magnitude = -restitutionMultiplier * relSpeed / sumInvMass;
            FlatVector collisionImpulse = Collision.magnitude * collisionDirection;
            bodyA.LinearAccelerate(-collisionImpulse * bodyA.InvMass);
            bodyB.LinearAccelerate(collisionImpulse * bodyB.InvMass);
        }
        private static void ResolveFrictionWithoutRotation(
            Body bodyA, Body bodyB, FlatVector collisionDirection, FlatVector relVelocity,
            double sumInvMass, double frictionStatic, double frictionDynamic)
        {
            FlatVector frictionDirection = relVelocity - (relVelocity * collisionDirection) * collisionDirection;
            if (frictionDirection % FlatVector.Zero) return;

            frictionDirection = frictionDirection.Unit();
            double frictionMagnitude = -relVelocity * frictionDirection / sumInvMass;

            FlatVector frictionImpulse;
            if (Math.Abs(frictionMagnitude) <= Collision.magnitude * frictionStatic) 
                frictionImpulse = frictionMagnitude * frictionDirection;
            else frictionImpulse = -Collision.magnitude * frictionDirection * frictionDynamic;

            bodyA.LinearAccelerate(-frictionImpulse * bodyA.InvMass);
            bodyB.LinearAccelerate(frictionImpulse * bodyB.InvMass);
        }



        private static void ResolveWithRotation(
            Body bodyA, Body bodyB, FlatVector normal)
        {
            int contactCount = Collision.GetContactPoints(bodyA, bodyB, out Collision.contactList[0], out Collision.contactList[1]);
            double sumInvMass = bodyA.InvMass + bodyB.InvMass;

            Collision.ResolveMovementWithRotation(
                bodyA, bodyB, contactCount, normal, sumInvMass,
                1 + Math.Min(bodyA.Restitution, bodyB.Restitution));

            double frictionStatic = bodyA.FrictionStatic + bodyB.FrictionStatic;
            double frictionDynamic = bodyA.FrictionDynamic + bodyB.FrictionDynamic;
            if (frictionStatic + frictionDynamic == 0) return;

            Collision.ResolveFrictionWithRotation(
                    bodyA, bodyB, contactCount, normal, sumInvMass,
                    FlatMath.Div2(frictionStatic), FlatMath.Div2(frictionDynamic));
        }
        private static void ResolveMovementWithRotation(
            Body bodyA, Body bodyB, 
            int contactCount, FlatVector collisionDirection,
            double sumInvMass, double restitutionMultiplier)
        {
            for (int i = 0; i < contactCount; i++)
            {
                FlatVector contact = contactList[i];
                FlatVector radiusPerpA = (contact - bodyA.Position).Normal;
                FlatVector radiusPerpB = (contact - bodyB.Position).Normal;
                double relSpeed = (
                    (radiusPerpB * bodyB.AngularVelocity + bodyB.LinearVelocity) -
                    (radiusPerpA * bodyA.AngularVelocity + bodyA.LinearVelocity)
                    ) * collisionDirection;

                if (relSpeed > 0) continue;

                double radialA = radiusPerpA * collisionDirection;
                double radialB = radiusPerpB * collisionDirection;
                double collisionMagnitude =
                    -restitutionMultiplier * relSpeed /
                    (
                    sumInvMass +
                    (radialA * radialA * bodyA.InvInertia) +
                    (radialB * radialB * bodyB.InvInertia)
                    ) /
                    contactCount;

                Collision.magnitudeList[i] = collisionMagnitude;
                FlatVector collisionImpulse = collisionMagnitude * collisionDirection;
                bodyA.LinearAccelerate(-collisionImpulse * bodyA.InvMass);
                bodyA.AngularAccelerate(-collisionImpulse * radiusPerpA * bodyA.InvInertia);
                bodyB.LinearAccelerate(collisionImpulse * bodyB.InvMass);
                bodyB.AngularAccelerate(collisionImpulse * radiusPerpB * bodyB.InvInertia);
            }
        }
        private static void ResolveFrictionWithRotation(
            Body bodyA, Body bodyB, 
            int contactCount, FlatVector collisionDirection, double sumInvMass, 
            double frictionStatic, double frictionDynamic)
        {
            for (int i = 0; i < contactCount; i++)
            {
                FlatVector contact = contactList[i];
                FlatVector radiusPerpA = (contact - bodyA.Position).Normal;
                FlatVector radiusPerpB = (contact - bodyB.Position).Normal;
                FlatVector relVelocity =
                    (radiusPerpB * bodyB.AngularVelocity + bodyB.LinearVelocity) -
                    (radiusPerpA * bodyA.AngularVelocity + bodyA.LinearVelocity);
                FlatVector frictionDirection = relVelocity - relVelocity * collisionDirection * collisionDirection;

                if (frictionDirection % FlatVector.Zero) return;

                frictionDirection = frictionDirection.Unit();
                double tangentialA = radiusPerpA * frictionDirection;
                double tangentialB = radiusPerpB * frictionDirection;
                double frictionMagnitude =
                    -relVelocity * frictionDirection /
                    (
                    sumInvMass +
                    (tangentialA * tangentialA * bodyA.InvInertia) +
                    (tangentialB * tangentialB * bodyB.InvInertia)
                    ) /
                    contactCount;

                double collisionMagnitude = Collision.magnitudeList[i];
                FlatVector frictionImpulse;
                if (Math.Abs(frictionMagnitude) <= collisionMagnitude * frictionStatic) 
                    frictionImpulse = frictionMagnitude * frictionDirection;
                else frictionImpulse = -collisionMagnitude * frictionDirection * frictionDynamic;

                bodyA.LinearAccelerate(-frictionImpulse * bodyA.InvMass);
                bodyA.AngularAccelerate(-frictionImpulse * radiusPerpA * bodyA.InvInertia);
                bodyB.LinearAccelerate(frictionImpulse * bodyB.InvMass);
                bodyB.AngularAccelerate(frictionImpulse * radiusPerpB * bodyB.InvInertia);
            }
        }
        


        // circle x polygon
        private static bool IsColliding(
            FlatVector polyCenter, FlatVector[] polyVertices,
            FlatVector circleCenter, double circleRadius,
            out double depth, out FlatVector normal)
        {
            normal = FlatVector.Zero;
            depth = double.MaxValue;
            FlatVector axis;
            double axisDepth, minPoly, maxPoly, minCircle, maxCircle;

            for (int i = 0; i < polyVertices.Length; i++)
            {
                FlatVector a = polyVertices[i];
                FlatVector b = polyVertices[(i + 1) % polyVertices.Length];
                axis = (b - a).Normal;

                Collision.ProjectVertices(polyVertices, axis, out minPoly, out maxPoly);
                Collision.ProjectVertices(circleCenter, circleRadius, axis, out minCircle, out maxCircle);

                if (minPoly >= maxCircle || minCircle >= maxPoly) return false;
                axisDepth = Math.Min(maxCircle - minPoly, maxPoly - minCircle);
                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }
            axis = Collision.GetContactPoint(circleCenter, polyVertices) - circleCenter;

            Collision.ProjectVertices(polyVertices, axis, out minPoly, out maxPoly);
            Collision.ProjectVertices(circleCenter, circleRadius, axis, out minCircle, out maxCircle);

            if (minPoly >= maxCircle || minCircle >= maxPoly) return false;
            axisDepth = Math.Min(maxCircle - minPoly, maxPoly - minCircle);
            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }

            double normalLength = normal.Length();
            depth /= normalLength;
            normal = normal.Unit(normalLength);
            if (((polyCenter - circleCenter) * normal) < 0) normal = -normal;
            return true;
        }
        private static FlatVector GetContactPoint(
            FlatVector circlePosition, FlatVector[] polygonVertices)
        {
            FlatVector contactPoint = FlatVector.Zero;
            double minDistanceSq = double.MaxValue;

            for (int i = 0; i < polygonVertices.Length; i++)
            {
                FlatVector a = polygonVertices[i];
                FlatVector b = polygonVertices[(i + 1) % polygonVertices.Length];
                FlatVector closestPoint = FlatVector.Project(circlePosition, a, b);
                double distanceSq = FlatVector.DistanceSquared(circlePosition, closestPoint);

                if (distanceSq >= minDistanceSq) continue;
                minDistanceSq = distanceSq;
                contactPoint = closestPoint;
            }
            return contactPoint;
        }



        // polygon x polygon
        private static bool IsColliding(
            FlatVector positionA, FlatVector[] verticesA,
            FlatVector positionB, FlatVector[] verticesB, 
            out double depth, out FlatVector normal)
        {
            normal = FlatVector.Zero;
            depth = double.MaxValue;

            for (int i = 0; i < verticesA.Length; i++)
            {
                FlatVector a = verticesA[i];
                FlatVector b = verticesA[(i + 1) % verticesA.Length];
                FlatVector axis = (b - a).Normal;

                Collision.ProjectVertices(verticesA, axis, out double minA, out double maxA);
                Collision.ProjectVertices(verticesB, axis, out double minB, out double maxB);

                if (minA >= maxB || minB >= maxA) return false;

                double axisDepth = Math.Min(maxB - minA, maxA - minB);
                if (axisDepth >= depth) continue;
                depth = axisDepth;
                normal = axis;
            }
            for (int i = 0; i < verticesB.Length; i++)
            {
                FlatVector a = verticesB[i];
                FlatVector b = verticesB[(i + 1) % verticesB.Length];
                FlatVector axis = (b - a).Normal;

                Collision.ProjectVertices(verticesA, axis, out double minA, out double maxA);
                Collision.ProjectVertices(verticesB, axis, out double minB, out double maxB);

                if (minA >= maxB || minB >= maxA) return false;

                double axisDepth = Math.Min(maxB - minA, maxA - minB);
                if (axisDepth >= depth) continue;
                depth = axisDepth;
                normal = axis;
            }
            double normalLength = normal.Length();
            depth /= normalLength;
            normal = normal.Unit(normalLength);
            if (((positionB - positionA) * normal) < 0) normal = -normal;
            return true;
        }
        private static int GetContactPoints(
            FlatVector[] verticesA, FlatVector[] verticesB, 
            out FlatVector contact1, out FlatVector contact2)
        {
            contact1 = FlatVector.Zero;
            contact2 = FlatVector.Zero;
            int contactCount = 0;
            double minDistanceSq = double.MaxValue;

            for (int i = 0; i < verticesA.Length; i++)
            {
                FlatVector p = verticesA[i];

                for (int j = 0; j < verticesB.Length; j++)
                {
                    FlatVector a = verticesB[j];
                    FlatVector b = verticesB[(j + 1) % verticesB.Length];
                    FlatVector closestPoint = FlatVector.Project(p, a, b);
                    double distanceSq = FlatVector.DistanceSquared(p, closestPoint);

                    if (distanceSq == minDistanceSq)
                    {
                        if (closestPoint == contact1 || 
                            closestPoint == contact2) continue;
                        contact2 = closestPoint;
                        contactCount = 2;
                    }
                    else if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        contact1 = closestPoint;
                        contactCount = 1;
                    }
                }
            }
            for (int i = 0; i < verticesB.Length; i++)
            {
                FlatVector p = verticesB[i];

                for (int j = 0; j < verticesA.Length; j++)
                {
                    FlatVector a = verticesA[j];
                    FlatVector b = verticesA[(j + 1) % verticesA.Length];
                    FlatVector closestPoint = FlatVector.Project(p, a, b);
                    double distanceSq = FlatVector.DistanceSquared(p, closestPoint);

                    if (distanceSq == minDistanceSq)
                    {
                        if (closestPoint == contact1 ||
                            closestPoint == contact2) continue;
                        contact2 = closestPoint;
                        contactCount = 2;
                    }
                    else if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        contact1 = closestPoint;
                        contactCount = 1;
                    }
                }
            }
            return contactCount;
        }
        private static void ProjectVertices(
            FlatVector[] vertices, FlatVector vector, 
            out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                double proj = vertices[i] * vector;
                if (proj < min) min = proj;
                if (proj > max) max = proj;
            }
        }



        // circle x circle
        private static bool IsColliding(
            FlatVector centerA, double radiusA,
            FlatVector centerB, double radiusB, 
            out double depth, out FlatVector normal)
        {
            depth = 0;
            normal = FlatVector.Zero;

            FlatVector relDisplacement = centerB - centerA;
            double distanceSq = relDisplacement.LengthSquared();
            double minDistance = radiusB + radiusA;
            double minDistanceSq = minDistance * minDistance;

            if (distanceSq >= minDistanceSq) return false;
            if (distanceSq == 0)
            {
                depth = minDistance;
                normal = new FlatVector(1, 0);
            }
            else
            {
                double distance = Math.Sqrt(distanceSq);
                depth = minDistance - distance;
                normal = relDisplacement.Unit(distance);
            }
            return true;
        }
        private static FlatVector GetContactPoint(
            FlatVector centerA, double radiusA,
            FlatVector centerB)
        {
            return centerA + (centerB - centerA).Unit() * radiusA;
        }
        private static void ProjectVertices(
            FlatVector center, double radius, FlatVector vector, 
            out double min, out double max)
        {
            FlatVector directionRadius = vector.Unit() * radius;
            min = (center + directionRadius) * vector;
            max = (center - directionRadius) * vector;
            if (min > max) (max, min) = (min, max);
        }
    }
}

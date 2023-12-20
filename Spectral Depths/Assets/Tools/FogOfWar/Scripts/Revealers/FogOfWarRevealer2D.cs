using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

namespace FOW
{
    public class FogOfWarRevealer2D : FogOfWarRevealer
    {
        RaycastHit2D[] InitialRayResults;
        
        protected override void _InitRevealer(int StepCount)
        {
            InitialRayResults = new RaycastHit2D[StepCount];
        }

        protected override void CleanupRevealer()
        {

        }

        protected override void IterationOne(int NumSteps, float firstAngle, float angleStep)
        {
            for (int i = 0; i < NumSteps; i++)
            {
                FirstIteration.RayAngles[i] = firstAngle + (angleStep * i);
                FirstIteration.Directions[i] = DirectionFromAngle(FirstIteration.RayAngles[i], true);
                RayHit = Physics2D.Raycast(EyePosition, FirstIteration.Directions[i], RayDistance, ObstacleMask);
                if (RayHit.collider != null)
                {
                    FirstIteration.Hits[i] = true;
                    FirstIteration.Normals[i] = RayHit.normal;
                    FirstIteration.Distances[i] = RayHit.distance;
                    FirstIteration.Points[i] = RayHit.point;
                }
                else
                {
                    FirstIteration.Hits[i] = false;
                    FirstIteration.Normals[i] = -FirstIteration.Directions[i];
                    FirstIteration.Distances[i] = RayDistance;
                    FirstIteration.Points[i] = GetPositionxy(EyePosition) + FirstIteration.Directions[i] * RayDistance;
                }
            }

            PointsJobHandle = PointsJob.Schedule(NumSteps, CommandsPerJob, default(JobHandle));
        }

        private RaycastHit2D RayHit;
        protected override void RayCast(float angle, ref SightRay ray)
        {
            Vector2 direction = DirectionFromAngle(angle, true);
            ray.angle = angle;
            ray.direction = direction;
            RayHit = Physics2D.Raycast(EyePosition, direction, RayDistance, ObstacleMask);

            if (RayHit.collider != null)
            {
                ray.hit = true;
                ray.normal = RayHit.normal;
                ray.distance = RayHit.distance;
                ray.point = RayHit.point;
            }
            else
            {
                ray.hit = false;
                ray.normal = -direction;
                ray.distance = RayDistance;
                ray.point = GetPositionxy(EyePosition) + ray.direction * RayDistance;
            }
        }

        private float2 pos2d;
        private float2 GetPositionxy(Vector3 pos)
        {
            pos2d.x = pos.x;
            pos2d.y = pos.y;
            return pos2d;
        }

        protected override void _FindEdge()
        {

        }

        protected override float GetEuler()
        {
            return transform.eulerAngles.z;
        }

        protected override Vector3 GetEyePosition()
        {
            return transform.position;
        }

        Vector3 hiderPosition;
        protected override void _RevealHiders()
        {
#if UNITY_EDITOR
            Profiler.BeginSample("Revealing Hiders");
#endif
            FogOfWarHider hiderInQuestion;
            float distToHider;
            float sightDist = ViewRadius;
            if (FogOfWarWorld.instance.UsingSoftening)
                sightDist += RevealHiderInFadeOutZonePercentage * SoftenDistance;
            for (int i = 0; i < FogOfWarWorld.numHiders; i++)
            {
                hiderInQuestion = FogOfWarWorld.hiders[i];
                bool seen = false;
                Transform samplePoint;
                float minDistToHider = distBetweenVectors(hiderInQuestion.transform.position, GetEyePosition()) - hiderInQuestion.maxDistBetweenPoints;
                if (minDistToHider < UnobscuredRadius || (minDistToHider < sightDist))
                {
                    for (int j = 0; j < hiderInQuestion.samplePoints.Length; j++)
                    {
                        samplePoint = hiderInQuestion.samplePoints[j];
                        distToHider = distBetweenVectors(samplePoint.position, GetEyePosition());
                        if (distToHider < UnobscuredRadius || (distToHider < sightDist && Mathf.Abs(AngleBetweenVector2(samplePoint.position - GetEyePosition(), getForward())) < ViewAngle / 2))
                        {
                            SetHiderPosition(samplePoint.position);
                            if (!Physics2D.Raycast(GetEyePosition(), hiderPosition - GetEyePosition(), distToHider, ObstacleMask))
                            {
                                seen = true;
                                break;
                            }
                        }
                    }
                }
                if (UnobscuredRadius < 0 && (minDistToHider + 1.5f * hiderInQuestion.maxDistBetweenPoints) < -UnobscuredRadius)
                    seen = false;

                if (seen)
                {
                    if (!hidersSeen.Contains(hiderInQuestion))
                    {
                        hidersSeen.Add(hiderInQuestion);
                        hiderInQuestion.AddSeer(this);
                    }
                }
                else
                {
                    if (hidersSeen.Contains(hiderInQuestion))
                    {
                        hidersSeen.Remove(hiderInQuestion);
                        hiderInQuestion.RemoveSeer(this);
                    }
                }
            }
#if UNITY_EDITOR
            Profiler.EndSample();
#endif
        }
        void SetHiderPosition(Vector3 point)
        {
            hiderPosition.x = point.x;
            hiderPosition.y = point.y;
            //hiderPosition.z = getEyePos().z;
        }
        protected override bool _TestPoint(Vector3 point)
        {
            float sightDist = ViewRadius;
            if (FogOfWarWorld.instance.UsingSoftening)
                sightDist += RevealHiderInFadeOutZonePercentage * SoftenDistance;

            float distToPoint = distBetweenVectors(point, GetEyePosition());
            if (distToPoint < UnobscuredRadius || (distToPoint < sightDist && Mathf.Abs(AngleBetweenVector2(point - GetEyePosition(), getForward())) < ViewAngle / 2))
            {
                SetHiderPosition(point);
                if (!Physics2D.Raycast(GetEyePosition(), hiderPosition - transform.position, distToPoint, ObstacleMask))
                    return true;
            }
            return false;
        }

        protected override void SetCenterAndHeight()
        {
            center.x = GetEyePosition().x;
            center.y = GetEyePosition().y;
            heightPos = transform.position.z;
        }

        Vector2 vec1;
        Vector2 vec2;
        Vector2 _vec1Rotated90;
        protected override float AngleBetweenVector2(Vector3 _vec1, Vector3 _vec2)
        {
            vec1.x = _vec1.x;
            vec1.y = _vec1.y;
            vec2.x = _vec2.x;
            vec2.y = _vec2.y;

            //vec1 = vec1.normalized;
            //vec2 = vec2.normalized;
            _vec1Rotated90.x = -vec1.y;
            _vec1Rotated90.y = vec1.x;
            //Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
            float sign = (Vector2.Dot(_vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
            return Vector2.Angle(vec1, vec2) * sign;
        }
        float distBetweenVectors(Vector3 _vec1, Vector3 _vec2)
        {
            vec1.x = _vec1.x;
            vec1.y = _vec1.y;
            vec2.x = _vec2.x;
            vec2.y = _vec2.y;
            return Vector2.Distance(vec1, vec2);
        }

        Vector3 getForward()
        {
            return new Vector3(-transform.up.x, transform.up.y, 0).normalized;
        }

        RaycastHit2D rayHit;

        Vector2 direction2d = Vector3.zero;
        Vector2 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.z;
            }
            direction2d.x = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            direction2d.y = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            return direction2d;
        }

        Vector3 direction = Vector3.zero;
        protected override Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.z;
            }
            direction.x = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            direction.y = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            return direction;
        }
    }
}

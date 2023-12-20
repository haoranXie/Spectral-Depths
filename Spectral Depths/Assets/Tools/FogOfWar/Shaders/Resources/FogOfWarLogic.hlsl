#ifndef _CONESETUP_
#define _CONESETUP_

struct CircleStruct
{
    float2 circleOrigin;
    int startIndex;
    int numSegments;
    float circleHeight;
    float unobscuredRadius;
    float circleRadius;
    float circleFade;
    float visionHeight;
    float heightFade;
    float revealerOpacity;
};
struct ConeEdgeStruct
{
    float edgeAngle;
    float length;
    bool cutShort;
};

#pragma multi_compile_local HARD SOFT
//#pragma multi_compile_local _ BLEED
bool BLEED;
#pragma multi_compile_local _ INNER_SOFTEN
//#pragma multi_compile_local FADE_EXP FADE_SMOOTH FADE_SMOOTHER FADE_SMOOTHSTEP
int _fadeType;
//#pragma multi_compile_local _ MIN_DIST_ON

//#pragma multi_compile_local BLEND_MAX BLEND_ADDITIVE
bool BLEND_MAX;
//#pragma multi_compile_local _ USE_WORLD_BOUNDS

#pragma multi_compile_local _ SAMPLE_REALTIME
//#pragma multi_compile_local _ SAMPLE_TEXTURE
#pragma multi_compile_local _ USE_TEXTURE_BLUR

float _extraRadius;

float _fadeOutDegrees;
//float _fadeOutDistance;
float _unboscuredFadeOutDistance;

int _NumCircles;
StructuredBuffer<int> _ActiveCircleIndices;
StructuredBuffer<CircleStruct> _CircleBuffer;
StructuredBuffer<ConeEdgeStruct> _ConeBuffer;
sampler2D _FowRT;
float4 _FowRT_TexelSize;
int _Sample_Blur_Quality;
float _Sample_Blur_Amount;
float4 _worldBounds;
float _worldBoundsSoftenDistance;
float _worldBoundsInfluence;

float lineThickness = .1;

//2D shenanigans
float _cameraSize;
float2 _cameraPosition;
float _cameraRotation;

//int _fadeType;
float _fadePower;

float Unity_InverseLerp_float4(float4 A, float4 B, float4 T)
{
    return (T - A) / (B - A);
}
//float SampleBlurredTexture(float2 UV, float Blur)
float SampleBlurredTexture(float2 UV, int quality)
{
    float Out_Alpha = 0;
    float kernelSum = 0.0;
    
    //Blur = min(Blur, 16);
    //int upper = ((Blur - 1) / 2);
    int upper = quality;
    int lower = -upper;
 
    [loop]
    for (int x = lower; x <= upper; ++x)
    {
        [loop]
        for (int y = lower; y <= upper; ++y)
        {
            kernelSum++;
 
            float2 offset = float2(_FowRT_TexelSize.x * x, _FowRT_TexelSize.y * y) * _Sample_Blur_Amount;
            Out_Alpha += 1 - tex2D(_FowRT, UV + offset).w;
        }
    }
 
    Out_Alpha /= kernelSum;
    return Out_Alpha;

}
void TextureSample(float2 Position, inout float coneOut)
{
#if SAMPLE_REALTIME
#else
    float2 uv = float2((((Position.x - _worldBounds.y) + (_worldBounds.x / 2)) / _worldBounds.x),
                 (((Position.y - _worldBounds.w) + (_worldBounds.z / 2)) / _worldBounds.z));
    
#if USE_TEXTURE_BLUR
    float texSamp = SampleBlurredTexture(uv, _Sample_Blur_Quality);
#else
    float texSamp = 1-tex2D(_FowRT, uv).w;
#endif
    if (Position.x > _worldBounds.y + (_worldBounds.x / 2) ||
        Position.x < _worldBounds.y - (_worldBounds.x / 2) ||
        Position.y > _worldBounds.w + (_worldBounds.z / 2) ||
        Position.y < _worldBounds.w - (_worldBounds.z / 2))
    {
        texSamp = 0;
    }
    
    //texSamp = Unity_InverseLerp_float4(0, .52, texSamp);
    //coneOut = float2(uv.x, 0);
    //coneOut = lerp(texSamp, coneOut, coneOut);
    coneOut = max(texSamp, coneOut);
    //coneOut += texSamp;
    coneOut = clamp(coneOut, 0, 1);
#endif
}
void CustomCurve_float(float In, out float Out)
{
    Out = In; //fade type 1; linear
    switch (_fadeType)
    {
        case 0: //Linear Fade
            return;
        case 1: //Smooth Fade
            Out = sin(Out * 1.570796);
            return;
        case 2: //Smoother Fade
            Out = .5 - (cos(Out * 3.1415926) * .5);
            return;
        case 3: //Smoothstep Fade
            Out = smoothstep(0, 1, In);
            return;
        case 4: //Exponential Fade
            Out = pow(Out, _fadePower);
            return;
    }
//#if FADE_SMOOTH
//    Out = sin(Out * 1.570796);
//#elif FADE_SMOOTHER
//    Out = .5 - (cos(Out * 3.1415926) * .5);
//#elif FADE_SMOOTHSTEP
//    Out = smoothstep(0, 1, In);
//#elif FADE_EXP
//    Out = pow(Out, _fadePower);
//#endif
}
void OutOfBoundsCheck(float2 Position, inout float4 color)
{
//#if USE_WORLD_BOUNDS
    float OOBX = max(0, ((Position.x + _worldBoundsSoftenDistance) - (_worldBounds.y + (_worldBounds.x / 2))));
    OOBX = max(OOBX, -(Position.x - _worldBoundsSoftenDistance - (_worldBounds.y - (_worldBounds.x / 2))));
    float OOBY = max(0, ((Position.y + _worldBoundsSoftenDistance) - (_worldBounds.w + (_worldBounds.z / 2))));
    OOBY = max(OOBY, -((Position.y - _worldBoundsSoftenDistance) - (_worldBounds.w - (_worldBounds.z / 2))));
    
    float OOB = length((float2(OOBX, OOBY)));
    OOB = saturate(OOB / _worldBoundsSoftenDistance);
    OOB *= _worldBoundsInfluence;
    CustomCurve_float(OOB, OOB);
    color = lerp(color, float4(0, 0, 0, 1), OOB * _worldBoundsInfluence);
    //if (Position.x > _worldBounds.y + (_worldBounds.x/2) ||
    //    Position.x < _worldBounds.y - (_worldBounds.x/2) ||
    //    Position.y > _worldBounds.w + (_worldBounds.z/2) ||
    //    Position.y < _worldBounds.w - (_worldBounds.z/2))
    //{
    //    color = lerp(color, float4(0, 0, 0, 1), _worldBoundsInfluence);
    //}
//#endif
}

float CalculateFadeZonePercent(float segmentLength, float SoftenDistance, float DistanceToOrigin)
{
    return ((segmentLength + SoftenDistance) - DistanceToOrigin) / SoftenDistance;
}
float SmoothValue(float val)
{
    //return val;
    return smoothstep(0, 1, val);
    //val = clamp(val, 0, 1);
    //return sin(val * 1.570796);;
    //return 1 - (cos(val * 3.14159) * .5 + .5);
}

float angleDiff(float ang1, float ang2)
{
    float diff = (ang1 - ang2 + 180) % 360 - 180;
    return diff > _fadeOutDegrees ? diff - 360 : diff;
}

void FOW_Hard_float(float2 Position, float height, out float Out)
{
    Out = 0;
#if SAMPLE_REALTIME
#else
    return;
#endif
    
    for (int i = 0; i < _NumCircles; i++)
    {
        CircleStruct circle = _CircleBuffer[_ActiveCircleIndices[i]];
        float distToCircleOrigin = distance(Position, circle.circleOrigin);
        if (distToCircleOrigin < circle.circleRadius)
        {
#if IGNORE_HEIGHT
            float heightDist = 0;
#else
            float heightDist = abs(height - circle.circleHeight);
#endif

            if (heightDist > circle.visionHeight)
                continue;

//#if MIN_DIST_ON
            if (circle.unobscuredRadius < 0 && distToCircleOrigin < -circle.unobscuredRadius)
                continue;
//#endif

            float2 relativePosition = Position - circle.circleOrigin;
            float deg = degrees(atan2(relativePosition.y, relativePosition.x));
            
            ConeEdgeStruct previousCone = _ConeBuffer[circle.startIndex];
            //float prevAng = previousCone.edgeAngle - .001;
            //float prevAng = previousCone.edgeAngle + .01;
            float prevAng = previousCone.edgeAngle;
            for (int c = 0; c < circle.numSegments; c++)
            {
                //prevAng = previousCone.edgeAngle - .001;
                //prevAng = previousCone.edgeAngle + .01;
                prevAng = previousCone.edgeAngle;
                ConeEdgeStruct currentCone = _ConeBuffer[circle.startIndex + c];

                float degDiff = angleDiff(deg + 360, currentCone.edgeAngle);
                float segmentAngle = currentCone.edgeAngle - prevAng;
                
                //if (deg > prevAng && currentCone.edgeAngle > deg)
                if (degDiff > -segmentAngle && degDiff < 0)
                {
                    //float lerpVal = (deg - prevAng) / (currentCone.edgeAngle - prevAng);
                    //float distToConeEnd = lerp(previousCone.length, currentCone.length, lerpVal);
                    float distToConeEnd = currentCone.length;
                    //if (abs(previousCone.length - circle.circleRadius) > .001 || abs(currentCone.length - circle.circleRadius) > .001)
                    if (previousCone.cutShort && currentCone.cutShort)
                    {
                        float2 start = circle.circleOrigin + float2(cos(radians(prevAng)), sin(radians(prevAng))) * previousCone.length;
                        float2 end = circle.circleOrigin + float2(cos(radians(currentCone.edgeAngle)), sin(radians(currentCone.edgeAngle))) * currentCone.length;
                        
                        float a1 = end.y - start.y;
                        float b1 = start.x - end.x;
                        float c1 = a1 * start.x + b1 * start.y;
                    
                        float a2 = Position.y - circle.circleOrigin.y;
                        float b2 = circle.circleOrigin.x - Position.x;
                        float c2 = a2 * circle.circleOrigin.x + b2 * circle.circleOrigin.y;
                    
                        float determinant = (a1 * b2) - (a2 * b1);
                    
                        float x = (b2 * c1 - b1 * c2) / determinant;
                        float y = (a1 * c2 - a2 * c1) / determinant;
                    
                        float2 intercection = float2(x, y);
                        distToConeEnd = distance(intercection, circle.circleOrigin) + _extraRadius;
                        
                        if (BLEED)
                        {
                            //to add the cone
                            float2 rotPoint = (start + end) / 2;
                            float2 arcOrigin = rotPoint + (float2(-(end.y - rotPoint.y), (end.x - rotPoint.x)) * 3);
                            float arcLength = distance(start, arcOrigin);
                            float2 newRelativePosition = arcOrigin + normalize(Position - arcOrigin) * arcLength;
                            distToConeEnd += distance(intercection, newRelativePosition) / 2;
                        }
                    }
                    distToConeEnd = max(distToConeEnd, circle.unobscuredRadius);
                    
                    if (distToCircleOrigin < distToConeEnd)
                    {
                        Out = circle.revealerOpacity;
                        return;
                    }
                }
                
                previousCone = currentCone;
            }
            if (distToCircleOrigin < circle.unobscuredRadius)
            {
                Out = circle.revealerOpacity;
                return;
            }
        }
    }
}

void FOW_Soft_float(float2 Position, float height, out float Out)
{
    Out = 0;
#if SAMPLE_REALTIME
#else
    return;
#endif
    for (int i = 0; i < _NumCircles; i++)
    {
        float RevealerOut = 0;
        CircleStruct circle = _CircleBuffer[_ActiveCircleIndices[i]];
        float distToCircleOrigin = distance(Position, circle.circleOrigin);
        float _fadeOutDistance = circle.circleFade;
        if (distToCircleOrigin < circle.circleRadius + _fadeOutDistance)
        {
#if IGNORE_HEIGHT
            float heightDist = 0;
#else
            float heightDist = abs(height - circle.circleHeight);
#endif

            if (heightDist > circle.visionHeight)
            {
                if (heightDist > circle.visionHeight + circle.heightFade)
                    continue;
                heightDist = 1 - (heightDist - circle.visionHeight) / circle.heightFade;
            }
            else
                heightDist = 1;

//#if MIN_DIST_ON
            if (circle.unobscuredRadius < 0)
            {
                if (distToCircleOrigin < -circle.unobscuredRadius + _unboscuredFadeOutDistance)
                {
                    if (distToCircleOrigin < -circle.unobscuredRadius)
                        continue;
                    //Out = max(Out, heightDist * lerp(1, 0, (distToCircleOrigin - circle.unobscuredRadius) / _unboscuredFadeOutDistance));
                    heightDist *= (distToCircleOrigin - -circle.unobscuredRadius) / _unboscuredFadeOutDistance;
                }
            }
//#endif
            float2 relativePosition = Position - circle.circleOrigin;
            float deg = degrees(atan2(relativePosition.y, relativePosition.x));
            //deg-=180;
            //Out = (deg)/360;
            //Out = 360%360;
            //return;
            
            //float degAdd = 0;
            ConeEdgeStruct previousCone = _ConeBuffer[circle.startIndex];
            //float prevAng = previousCone.edgeAngle - .001;
            //float prevAng = previousCone.edgeAngle + .01;
            float prevAng = previousCone.edgeAngle;
            for (int c = 0; c < circle.numSegments; c++)
            {
                //prevAng = previousCone.edgeAngle - .001;
                //prevAng = previousCone.edgeAngle + .01;
                prevAng = previousCone.edgeAngle;
                ConeEdgeStruct currentCone = _ConeBuffer[circle.startIndex + c];

                float degDiff = angleDiff(deg + 360, currentCone.edgeAngle);
                float segmentAngle = currentCone.edgeAngle - prevAng;
#if INNER_SOFTEN
                if (degDiff > -segmentAngle - _fadeOutDegrees && degDiff < _fadeOutDegrees)
#else
                if (degDiff > -segmentAngle && degDiff < 0)
#endif
                {
                    //float lerpVal = clamp(segmentAngle-degDiff, 0, segmentAngle)/segmentAngle;
                    //float distToConeEnd = lerp(previousCone.length, currentCone.length, lerpVal);
                    float distToConeEnd = currentCone.length;
                    float newBlurDistance = (distToConeEnd / circle.circleRadius) * _fadeOutDistance;
                    newBlurDistance = _fadeOutDistance;
                    
#if INNER_SOFTEN
                    if (!(degDiff > -segmentAngle && degDiff < 0))
                    {
                        float softDistToConeEnd = distToConeEnd;
                        float softnewBlurDistance = newBlurDistance;

                        float angDiff = degDiff / _fadeOutDegrees;
                        if (degDiff < 0)
                        {
                            angDiff = clamp(((segmentAngle - degDiff) / _fadeOutDegrees), 0, 1);
                        }
                        //float arcLen = (2 * (distToConeEnd * distToConeEnd)) - (2 * distToConeEnd * distToConeEnd * cos(radians(_fadeOutDegrees)));
                        
                        if (previousCone.cutShort)
                        {
                        
                            if (currentCone.cutShort)
                            {
                                softnewBlurDistance = 0;
                                //softDistToConeEnd = 0;
                            }
                            if ((c == 0 || c == circle.numSegments - 1))
                            {
                                softnewBlurDistance = distToConeEnd - circle.circleRadius;
                                softDistToConeEnd = circle.circleRadius;
                            }
                            
                            if (distToConeEnd > circle.circleRadius)
                            {
                                softnewBlurDistance = distToConeEnd - circle.circleRadius;
                                softDistToConeEnd = circle.circleRadius;
                            }
                            //if (currentCone.cutShort && !(c == 0 || c == circle.numSegments-1))
                            //{
                                //softnewBlurDistance = 0;
                                //softDistToConeEnd = 0;
                            //}
                        }
                        else
                        {
                            softDistToConeEnd = min(previousCone.length, currentCone.length);
                        }
                        //softnewBlurDistance+= arcLen;

                        if (distToCircleOrigin < softDistToConeEnd + softnewBlurDistance)
                        {
                            //if (distToCircleOrigin < softDistToConeEnd)
                            //{
                            //    //RevealerOut = max(RevealerOut, heightDist * cos(angDiff * 1.570796));
                            //    RevealerOut = max(RevealerOut, heightDist * SmoothValue((1-angDiff)));
                            //}
                            //else
                            ////RevealerOut = max(RevealerOut, heightDist * lerp(0, cos(angDiff * 1.570796), clamp(((softDistToConeEnd + _fadeOutDistance) - distToCircleOrigin) / _fadeOutDistance, 0, 1)));
                    
                            float b = CalculateFadeZonePercent(softDistToConeEnd, _fadeOutDistance, distToCircleOrigin);
                            float x = (1-angDiff);
                            RevealerOut = max(RevealerOut, heightDist * SmoothValue(b) * SmoothValue(x) );
                        }
                        previousCone = currentCone;
                        continue;
                    }
#endif
                    //Out = 1;
                    //previousCone = currentCone;
                    //continue;
                    //if (abs(previousCone.length - circle.circleRadius) > .001 || abs(currentCone.length - circle.circleRadius) > .001)
                    if (previousCone.cutShort && currentCone.cutShort)
                    {
                        float2 start = circle.circleOrigin + float2(cos(radians(prevAng)), sin(radians(prevAng))) * previousCone.length;
                        float2 end = circle.circleOrigin + float2(cos(radians(currentCone.edgeAngle)), sin(radians(currentCone.edgeAngle))) * currentCone.length;
                        
                        float a1 = end.y - start.y;
                        float b1 = start.x - end.x;
                        float c1 = a1 * start.x + b1 * start.y;
                    
                        float a2 = Position.y - circle.circleOrigin.y;
                        float b2 = circle.circleOrigin.x - Position.x;
                        float c2 = a2 * circle.circleOrigin.x + b2 * circle.circleOrigin.y;
                    
                        float determinant = (a1 * b2) - (a2 * b1);
                    
                        float x = (b2 * c1 - b1 * c2) / determinant;
                        float y = (a1 * c2 - a2 * c1) / determinant;
                    
                        float2 intercection = float2(x, y);
                        distToConeEnd = distance(intercection, circle.circleOrigin);
                        newBlurDistance = 0;
                        if (distToConeEnd > circle.circleRadius)
                        {
                            newBlurDistance = distToConeEnd - circle.circleRadius;
                            distToConeEnd = circle.circleRadius;
                        }
                        distToConeEnd += _extraRadius;
                        newBlurDistance += _extraRadius;
                        
                        if (BLEED)
                        {
                            //to add the cone
                            float2 rotPoint = (start + end) / 2;
                            float2 arcOrigin = rotPoint + (float2(-(end.y - rotPoint.y), (end.x - rotPoint.x)) * 3);
                            float arcLength = distance(start, arcOrigin);
                            float2 newRelativePosition = arcOrigin + normalize(Position - arcOrigin) * arcLength;
                            newBlurDistance += distance(intercection, newRelativePosition) / 2;
                        }
                    }
                    distToConeEnd = max(distToConeEnd, circle.unobscuredRadius);
                    distToConeEnd = min(distToConeEnd, circle.circleRadius);
                    
                    if (distToCircleOrigin < distToConeEnd + newBlurDistance)
                    {
                        //if (distToCircleOrigin < distToConeEnd)
                        //{
                        //    RevealerOut = 1 * heightDist;
                        //    break;
                        //}
                        //RevealerOut = max(RevealerOut, heightDist * ((distToConeEnd + _fadeOutDistance) - distToCircleOrigin) / _fadeOutDistance);
                        RevealerOut = max(RevealerOut, heightDist * SmoothValue(CalculateFadeZonePercent(distToConeEnd, _fadeOutDistance, distToCircleOrigin)));
                        previousCone = currentCone;
                        continue;
                    }
                }
                
                previousCone = currentCone;
            }
            //if (distToCircleOrigin < circle.unobscuredRadius)
            //{
            //    Out = 1;
            //    return;
            //}
            if (distToCircleOrigin < circle.unobscuredRadius + _unboscuredFadeOutDistance)
            {
                if (distToCircleOrigin < circle.unobscuredRadius)
                {
                    RevealerOut = 1 * heightDist;
                        //break;
                }
                else
                    RevealerOut = max(RevealerOut, heightDist * lerp(1, 0, (distToCircleOrigin - circle.unobscuredRadius) / _unboscuredFadeOutDistance));
            }
        }
        RevealerOut = clamp(abs(RevealerOut), 0, 1);
        CustomCurve_float(RevealerOut, RevealerOut);
        if (BLEND_MAX)
            Out = max(Out, RevealerOut);
        else
            Out = min(1, Out + RevealerOut);
//#if BLEND_MAX
//        Out = max(Out, RevealerOut);
//#elif BLEND_ADDITIVE
//        Out = min(1, Out + RevealerOut);
//#endif
    }
}

float2 closestPointOnLine(float2 p1, float2 p2, float2 pnt)
{
    float2 direction = normalize(p1 - p2);
    float2 vec = pnt - p1;
    float dst = dot(vec, direction);
    return p1 + direction * dst;
}

void FOW_Outline_float(float2 Position, float height, out float Out)
{
    Out = 0;
#if SAMPLE_REALTIME
#else
    return;
#endif
    
    for (int i = 0; i < _NumCircles; i++)
    {
        CircleStruct circle = _CircleBuffer[_ActiveCircleIndices[i]];
        float distToCircleOrigin = distance(Position, circle.circleOrigin);
        if (distToCircleOrigin < circle.circleRadius + lineThickness)
        {
#if IGNORE_HEIGHT
            float heightDist = 0;
#else
            float heightDist = abs(height - circle.circleHeight);
#endif

            if (heightDist > circle.visionHeight)
                continue;

//#if MIN_DIST_ON
            if (circle.unobscuredRadius < 0 && distToCircleOrigin < -circle.unobscuredRadius)
                continue;
//#endif

            float2 relativePosition = Position - circle.circleOrigin;
            float deg = degrees(atan2(relativePosition.y, relativePosition.x));
            
            ConeEdgeStruct previousCone = _ConeBuffer[circle.startIndex];
            //float prevAng = previousCone.edgeAngle - .001;
            //float prevAng = previousCone.edgeAngle + .01;
            float prevAng = previousCone.edgeAngle;
            for (int c = 0; c < circle.numSegments; c++)
            {
                //prevAng = previousCone.edgeAngle - .001;
                //prevAng = previousCone.edgeAngle + .01;
                prevAng = previousCone.edgeAngle;
                ConeEdgeStruct currentCone = _ConeBuffer[circle.startIndex + c];

                if (previousCone.cutShort != currentCone.cutShort)
                {
                    float2 previousPoint = circle.circleOrigin + float2(previousCone.length * cos(radians(previousCone.edgeAngle)), previousCone.length * sin(radians(previousCone.edgeAngle)));
                    float2 currentPoint = circle.circleOrigin + float2(currentCone.length * cos(radians(currentCone.edgeAngle)), currentCone.length * sin(radians(currentCone.edgeAngle)));
                
                    float len = distance(previousPoint, currentPoint) + lineThickness;
                    float dstTop1 = distance(previousPoint, Position);
                    float dstTop2 = distance(currentPoint, Position);
                
                    float2 ClosestPointOnLine = closestPointOnLine(previousPoint, currentPoint, Position);
                    float dstToLine = distance(ClosestPointOnLine, Position);
                
                //dst = distance(currentPoint, Position);
                    if (dstToLine < lineThickness && dstTop1 < len && dstTop2 < len)
                    {
                        Out = 1;
                        return;
                    }
                }
                float degDiff = angleDiff(deg + 360, currentCone.edgeAngle);
                float segmentAngle = currentCone.edgeAngle - prevAng;
                
                //if (deg > prevAng && currentCone.edgeAngle > deg)
                if (degDiff > -segmentAngle && degDiff < 0)
                {
                    //float lerpVal = (deg - prevAng) / (currentCone.edgeAngle - prevAng);
                    //float distToConeEnd = lerp(previousCone.length, currentCone.length, lerpVal);
                    float distToConeEnd = currentCone.length;
                    //if (abs(previousCone.length - circle.circleRadius) > .001 || abs(currentCone.length - circle.circleRadius) > .001)
                    if (previousCone.cutShort && currentCone.cutShort)
                    {
                        float2 start = circle.circleOrigin + float2(cos(radians(prevAng)), sin(radians(prevAng))) * previousCone.length;
                        float2 end = circle.circleOrigin + float2(cos(radians(currentCone.edgeAngle)), sin(radians(currentCone.edgeAngle))) * currentCone.length;
                        
                        float a1 = end.y - start.y;
                        float b1 = start.x - end.x;
                        float c1 = a1 * start.x + b1 * start.y;
                    
                        float a2 = Position.y - circle.circleOrigin.y;
                        float b2 = circle.circleOrigin.x - Position.x;
                        float c2 = a2 * circle.circleOrigin.x + b2 * circle.circleOrigin.y;
                    
                        float determinant = (a1 * b2) - (a2 * b1);
                    
                        float x = (b2 * c1 - b1 * c2) / determinant;
                        float y = (a1 * c2 - a2 * c1) / determinant;
                    
                        float2 intercection = float2(x, y);
                        distToConeEnd = distance(intercection, circle.circleOrigin);
                    }
                    distToConeEnd = max(distToConeEnd, circle.unobscuredRadius);
                    
                    if (distance(distToCircleOrigin, distToConeEnd) < lineThickness)
                    {
                        Out = 1;
                        return;
                    }
                }
                
                previousCone = currentCone;
            }
        }
    }
}

//shadergraph rotate node
void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    Rotation = Rotation * (3.1415926f / 180.0f);
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;
    Out = UV;
}
#endif
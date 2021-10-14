Shader "Custom/s_NoiseCustom"
{
    Properties
    {
        _speedX("Speed X axis", float) = 0.1
        _speedY("Speed Y axis", float) = 0.0
        _value("Value", Range( -2.0 , 2.0)) = 0.0
        _amplitude("Amplitude", Range( 0.0 , 5.0)) = 1.5
        _range("Range", Range(0.0, 1.0)) = 0.0
        _frequency("Frequency", Range( 0.0 , 6.0)) = 2.0
        _power("Power", Range( 0.1 , 5.0)) = 1.0
        _scale("Scale", Float) = 1.0    
        [HDR] _emissionColor("Color", Color) = (0,0,0)
        _emission("Emission Strength", Range(0.1, 5.0)) = 0
    }
    Subshader
    {
        Tags { 
            "RenderType" ="Transparent" 
            "Queue"="Transparent"
        }
        
        Cull Off
        Blend One One
        ZWrite Off  

        Pass
        {
            CGPROGRAM
            #pragma vertex Interpolators
            #pragma fragment frag
            #pragma target 3.0
           
            struct MeshData
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            float _octaves,_lacunarity,_gain,_value,_amplitude,_frequency, _offsetX, _offsetY, 
            _power, _scale, _range, _emission;
            float4 _color;
            fixed4 _emissionColor;
            float _speedX, _speedY;
           
            float noiseCreator( float2 p )
            {
                p = p * _scale + float2(_offsetX + (_Time.y * _speedX),_offsetY + (_Time.y * _speedY));
                float2 i = floor( p * _frequency);
                float2 f = frac( p * _frequency );   
                float2 t = f * f * f * ( f * ( f * 6.0 - 15.0 ) + 10.0 );
                float2 a = i + float2( 0.0, 0.0 );
                float2 b = i + float2( 1.0, 0.0 );
                float2 c = i + float2( 0.0, 1.0 );
                float2 d = i + float2( 1.0, 1.0 );
                a = -1.0 + 2.0 * frac( sin( float2( dot( a, float2( 127.1, 311.7 ) ),dot( a, float2( 269.5,183.3 ) ) ) ) * 43758.5453123 );
                b = -1.0 + 2.0 * frac( sin( float2( dot( b, float2( 127.1, 311.7 ) ),dot( b, float2( 269.5,183.3 ) ) ) ) * 43758.5453123 );
                c = -1.0 + 2.0 * frac( sin( float2( dot( c, float2( 127.1, 311.7 ) ),dot( c, float2( 269.5,183.3 ) ) ) ) * 43758.5453123 );
                d = -1.0 + 2.0 * frac( sin( float2( dot( d, float2( 127.1, 311.7 ) ),dot( d, float2( 269.5,183.3 ) ) ) ) * 43758.5453123 );
                float A = dot( a, f - float2( 0.0, 0.0 ) );
                float B = dot( b, f - float2( 1.0, 0.0 ) );
                float C = dot( c, f - float2( 0.0, 1.0 ) );
                float D = dot( d, f - float2( 1.0, 1.0 ) );
                float noise = ( lerp( lerp( A, B, t.x ), lerp( C, D, t.x ), t.y ) );   
                _value += _amplitude * noise;
                _value = clamp( _value, -1.0, 1.0 );
                return pow(_value * 0.5 + 0.5,_power);
            }
           
            MeshData Interpolators (float4 vertex:POSITION, float2 uv:TEXCOORD0)
            {
                MeshData vs;
                vs.vertex = UnityObjectToClipPos (vertex);
                vs.uv = uv;
                return vs;
            }
 
            float4 frag (MeshData o) : SV_TARGET
            {  
                float2 uv = o.uv.xy;
                float c = noiseCreator(uv);
                return float4(c, c, c, c) * _emission * _emissionColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

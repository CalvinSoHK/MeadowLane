// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CameraDefaultShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MidPoint("Midpoint", Range(0.0, 1.0)) = 0.5
		_LumMidPoint("Luminosity Midpoint", Range(0, 0.1)) = 1
		_Strength("Contrast Strength", Range(0.0, 10)) = 0.2
		_Saturation("Saturation", Range(0.0, 10)) = 0.2
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _MidPoint;
			float _Strength;
			float _Saturation;
			float _LumMidPoint;

			//Helper functions
			//RGB to HSV function
			float3 rgb_to_hsl(float3 RGB) 
			{
				//Calculate HSL from rgb
				float hue, sat, lum;
				float red = RGB.x;
				float green = RGB.y;
				float blue = RGB.z;

				//1. Find min and max of rgb values.
				float minValue, maxValue;
				minValue = min(red, green);
				minValue = min(minValue, blue);

				//Note: can't compare more than two
				maxValue = max(red, green);
				maxValue = max(maxValue, blue);

				//2. Find luminace value. Add both min and max and divide by two.
				lum = (minValue + maxValue) / 2;

				//3. Find saturation. 
				//If min and max are the samwe we have no saturation.
				if (minValue == maxValue) {
					hue = 0;
					sat = 0;
				}//Else we need to check our brightness to see which formula.
				else if (lum < 0.5f) {
					sat = ((maxValue - minValue) / (maxValue + minValue));
				}
				else if (lum >= 0.5f) {
					sat = ((maxValue - minValue) / (2.0 - maxValue - minValue));
				}

				//4. Find hue, formula based on which RGB is maxed
				if (maxValue == red) {
					hue = (green - blue) / (maxValue - minValue);
				}
				else if (maxValue == green) {
					hue = 2.0 + (blue - red) / (maxValue - minValue);
				}
				else if (maxValue == blue) {
					hue = 4.0 + (red - green) / (maxValue - minValue);
				}

				//Convert to a circle by multiplying by 60.
				hue *= 60;

				//If we are negative add 360 to it since it is a circle.
				if (hue < 0) {
					hue += 360;
				}
				
				return float3(hue, sat, lum);
			}

			//HSV to RGB function
			float3 hsl_to_rgb(float3 HSL) {

				//Convert HSL back to RGB.
				float temp1, temp2, red, green, blue;
				float hue = HSL.x;
				float sat = HSL.y;
				float lum = HSL.z;


				//1. Calculate the temp1 value based on lum.
				if (lum <= 0.5f) {
					temp1 = lum * (1.0 + sat);
				}
				else if (lum > 0.5f) {
					temp1 = lum + sat - lum * sat;
				}

				//2. Calculate the temp2 value.
				temp2 = 2 * lum - temp1;

				//3. Convert hue to 0.0 - 1.0
				hue = hue / 360;

				//4. Calculate more temp values.
				float tempR, tempG, tempB;
				tempR = hue + 0.333;
				if (tempR < 0) {
					tempR += 1;
				}
				else if (tempR > 1) {
					tempR -= 1;
				}

				tempG = hue;
				if (tempG < 0) {
					tempG += 1;
				}
				else if (tempG > 1) {
					tempG -= 1;
				}

				tempB = hue - 0.333;
				if (tempB < 0) {
					tempB += 1;
				}
				else if (tempB > 1) {
					tempB -= 1;
				}

				//5. Calculate the RGB values.
				if (6 * tempR < 1) {
					red = temp2 + (temp1 - temp2) * 6 * tempR;
				}
				else if (2 * tempR < 1) {
					red = temp1;
				}
				else if (3 * tempR < 2) {
					red = temp2 + (temp1 - temp2) * (0.666 - tempR) * 6;
				}
				else {
					red = temp2;
				}

				if (6 * tempG < 1) {
					green = temp2 + (temp1 - temp2) * 6 * tempG;
				}
				else if (2 * tempG < 1) {
					green = temp1;
				}
				else if (3 * tempG < 2) {
					green = temp2 + (temp1 - temp2) * (0.666 - tempG) * 6;
				}
				else {
					green = temp2;
				}

				if (6 * tempB < 1) {
					blue = temp2 + (temp1 - temp2) * 6 * tempB;
				}
				else if (2 * tempB < 1) {
					blue = temp1;
				}
				else if (3 * tempB < 2) {
					blue = temp2 + (temp1 - temp2) * (0.666 - tempB) * 6;
				}
				else {
					blue = temp2;
				}

				return float3(red, green, blue);
			}

			fixed4 frag (v2f i) : SV_Target
			{


				fixed4 col = tex2D(_MainTex, i.uv);
				
				//get hsl. then modify.
				float3 hsl = rgb_to_hsl(col);
				//Do something to the HSL values
				float lum = hsl.z;

				//For now assume midpoint is the literal mid value.
				float difference = lum - _MidPoint;

				if(lum < _LumMidPoint){
					lum *= -_Strength;
				}
				else{
				    lum *= _Strength;
				}
			
				//lum += difference * _Strength;

				//Bounding checks.
				if (lum > 1) {
					lum = 1;
				}
				else if (lum < 0) {
					lum = 0;
				}

				//Do something to Saturation
				float sat = hsl.y;
				sat *= _Saturation * (1 - lum);
				
				hsl.y = sat;

				hsl.z = lum;
				//Write a formula to is smoother
	
				float3 rgb = hsl_to_rgb(hsl);
				col.r = rgb.x;
				col.g = rgb.y;
				col.b = rgb.z;

				return col;
			}
			ENDCG
		}
	}
}
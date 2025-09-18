// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1) // ���� ��������� (������ �� ���������)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.05 // ������� ���������
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" } // �������������� ����� ������� ��������
        LOD 100

        // ��������� ��������� ������ ������, ����� ������ ��� ��������
        Cull Front // ������������ ������ ������ �����

        ZWrite Off // �� ���������� � Z-�����, ����� ������ ��� ������
        Blend SrcAlpha OneMinusSrcAlpha // ��� ������������

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL; // ����� ������� ��� ��������
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR; // ���� �������
            };

            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                // ������� ������� ����� �� �������, ����� ������� "�������"
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal); // �������������� ������� � ������� ����������

                worldPos.xyz += worldNormal * _OutlineWidth; // �������� �� �������

                o.vertex = UnityObjectToClipPos(worldPos);
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Standard" // ���� ���-�� ������ �� ���, ������������ ����������� ������
}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1) // Цвет подсветки (желтый по умолчанию)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.05 // Толщина подсветки
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" } // Отрисовывается после обычных объектов
        LOD 100

        // Отключаем отсечение задних граней, чтобы видеть все полигоны
        Cull Front // Отрисовываем только задние грани

        ZWrite Off // Не записываем в Z-буфер, чтобы контур был поверх
        Blend SrcAlpha OneMinusSrcAlpha // Для прозрачности

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL; // Нужны нормали для смещения
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR; // Цвет контура
            };

            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                // Смещаем вершину вдоль ее нормали, чтобы создать "толщину"
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal); // Преобразование нормали в мировые координаты

                worldPos.xyz += worldNormal * _OutlineWidth; // Смещение по нормали

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
    FallBack "Standard" // Если что-то пойдет не так, использовать стандартный шейдер
}
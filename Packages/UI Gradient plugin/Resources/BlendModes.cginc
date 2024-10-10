fixed4 Darken (fixed4 top, fixed4 base)
{ 
    fixed4 color = min(top, base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 Multiply (fixed4 top, fixed4 base)
{ 
    fixed4 color = top * base;
    color = lerp(base,color,top.w);
    return color;
}

fixed4 ColorBurn (fixed4 top, fixed4 base) 
{ 
    fixed4 color = 1.0 - (1.0 - top) / base;
    color = lerp(base,color,top.w);
    return color;
}

fixed4 LinearBurn (fixed4 top, fixed4 base)
{ 
    fixed4 color = top + base - 1.0;
    color = lerp(base,color,top.w);
    return color;
}

fixed luminance (fixed4 color) 
{ 
    return .299 * color.r + .587 * color.g + .114 * color.b; 
}

fixed4 DarkerColor (fixed4 top, fixed4 base) 
{ 
    fixed4 color = luminance(top) < luminance(base) ? top : base;
    color = lerp(base,color,top.w);
    return color; 
}   

fixed4 Lighten (fixed4 top, fixed4 base)
{ 
    fixed4 color = max(top, base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 Screen (fixed4 top, fixed4 base) 
{ 	
    fixed4 color = 1.0 - (1.0 - top) * (1.0 - base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 ColorDodge (fixed4 top, fixed4 base) 
{ 
    fixed4 color = top / (1.0 - base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 LinearDodge (fixed4 top, fixed4 base)
{ 
    fixed4 color = top + base;
    color = lerp(base,color,top.w);
    return color;
} 

fixed4 LighterColor (fixed4 top, fixed4 base) 
{ 
    fixed4 color = luminance(top) > luminance(base) ? top : base;
    color = lerp(base,color,top.w);
    return color; 
}

fixed4 Overlay (fixed4 top, fixed4 base) 
{
    fixed4 color = top > .5 ? 1.0 - 2.0 * (1.0 - top) * (1.0 - base) : 2.0 * top * base;
    color = lerp(base,color,top.w);
    return color;
}

fixed4 SoftLight (fixed4 top, fixed4 base)
{
    fixed4 color = (1.0 - top) * top * base + top * (1.0 - (1.0 - top) * (1.0 - base));
    color = lerp(base,color,top.w);
    return color;
}

fixed4 HardLight (fixed4 top, fixed4 base)
{
    fixed4 color = base > .5 ? 1.0 - (1.0 - top) * (1.0 - 2.0 * (base - .5)) : top * (2.0 * base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 VividLight (fixed4 top, fixed4 base)
{
    fixed4 color = base > .5 ? top / (1.0 - (base - .5) * 2.0) : 1.0 - (1.0 - top) / (base * 2.0);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 LinearLight (fixed4 top, fixed4 base)
{
    fixed4 color = base > .5 ? top + 2.0 * (base - .5) : top + 2.0 * base - 1.0;
    color = lerp(base,color,top.w);
    return color;
}

fixed4 PinLight (fixed4 top, fixed4 base)
{
    fixed4 color = base > .5 ? max(top, 2.0 * (base - .5)) : min(top, 2.0 * base);
    color = lerp(base,color,top.w);
    return color;
}

fixed4 HardMix (fixed4 top, fixed4 base)
{
    fixed4 color = (base > 1.0 - top) ? 1.0 : .0;
    color = lerp(base,color,top.w);
    return color;
}

fixed4 Difference (fixed4 top, fixed4 base) 
{ 
    fixed4 color = abs(top - base);
    color = lerp(base,color,top.w);
    return color; 
}

fixed4 Exclusion (fixed4 top, fixed4 base)
{ 
    fixed4 color = top + base - 2.0 * top * base;
    color = lerp(base,color,top.w);
    return color; 
}

fixed4 Subtract (fixed4 top, fixed4 base)
{ 
    fixed4 color = top - base;
    color = lerp(base,color,top.w);
    return color; 
}

fixed4 Divide (fixed4 top, fixed4 base)
{ 
    fixed4 color = top / base;
    color = lerp(base,color,top.w);
    return color; 
}


fixed4 BlendColors (fixed4 top, fixed4 base) 
{ 	
    #ifdef BM_NORMAL
        return lerp(base, top, top.w);
    #elif BM_DARKEN 
        return Darken(top, base);
    #elif BM_MULTIPLY  
        return Multiply(top, base);
    #elif BM_LINEARBURN 
        return LinearBurn(top, base);
    #elif BM_DARKERCOLOR
        return DarkerColor(top, base);
    #elif BM_COLORBURN
        return ColorBurn(top, base);
    #elif BM_LIGHTEN 
        return Lighten(top, base);                
    #elif BM_SCREEN 
        return Screen(top, base);                                
    #elif BM_COLORDODGE 
        return ColorDodge(top, base);                                
    #elif BM_LINEARDODGE 
        return LinearDodge(top, base);                                
    #elif BM_LIGHTENCOLOR 
        return LighterColor(top, base);                
    #elif BM_OVERLAY 
        return Overlay(top, base);                
    #elif BM_SOFTLIGHT 
        return SoftLight(top, base);                
    #elif BM_HARDLIGHT 
        return HardLight(top, base);                
    #elif BM_VIVIDLIGHT 
        return VividLight(top, base);                
    #elif BM_LINEARLIGHT 
        return LinearLight(top, base);                
    #elif BM_PINLIGHT 
        return PinLight(top, base);                
    #elif BM_HARDMIX 
        return HardMix(top, base);                
    #elif BM_DIFFERENCE 
        return Difference(top, base);                
    #elif BM_EXCLUSION 
        return Exclusion(top, base);                
    #elif BM_SUBTRACT 
        return Subtract(top, base);                
    #elif BM_DIVIDE
        return Divide(top, base);                
    #endif
    return top;
}
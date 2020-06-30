# "Functionalized" by Xlfdll

function removeSubtitles(clip clip, int topLeftX, int topLeftY)
{
clip = clip.ConvertToRGB32()
maskclip = Levels(clip, 111, 10.000, 255, 99, 211)
maskclip = GeneralConvolution(maskclip, 0, "
   10 10 10 10 10
   10 10 10 10 10 
   10 10 10 10 10 
   10 10 10 10 10 
   10 10 10 10 10 ", 256, false)
clip2 = OverLay(clip,maskclip,0,0,mode="subtract")
clip2 = Blur(clip2, 1.58)
clip2 = ConvertToYUY2(clip2)
clip2= Tweak(clip2, hue=-180.0,bright=22,cont=10.0)
maskclip = Crop(clip2, topLeftX,topLeftY,-topLeftX,0).ConvertToRGB32.crop(0,0,0,-2).Trim(0,0)
maskclip = maskclip.Brightness_Contrast(-254, 32) 
maskclip = ConvertToYUY2(maskclip)
maskclip = Tweak(maskclip, hue=-180.0,sat=0.0,bright=22,cont=10.0)
maskclip = ConvertToRGB32(maskclip)
maskclip = maskclip.Levels(0, 0.281, 100, 6, 255)
maskclip = maskclip.Greyscale()
maskclip = maskclip.Exorcist(2, 100) 
maskclip = maskclip.Exorcist(-2, 100) 
maskclip = maskclip.Brightness_Contrast(-21, 32) 
maskclip = GeneralConvolution(maskclip, 0, "
   25 25 25 25 25
   25 25 25 25 25
   25 25 25 25 25
   25 25 25 25 25
   25 25 25 25 25   ", 256, false) 
maskclip = maskclip.Core(116,0)
maskclip = maskclip.Blur(1.58)
clip = dekafka(clip, maskclip, topLeftX, topLeftY, maskclip.width, maskclip.height, 256)
clip = ConvertToYV12(clip)
return clip
}

function removeSubtitlesOnDuration(clip, int topLeftX, int topLeftY, int startFrame, int endFrame)
{
startClip = Trim(clip, 0, startFrame - 1, false)
workClip = Trim(clip, startFrame, endFrame, false)
endClip = Trim(clip, endFrame + 1, 0, false)

workClip = removeSubtitles(workClip, topLeftX, topLeftY)

return ConvertToYV12(startClip) + workClip + ConvertToYV12(endClip)
}
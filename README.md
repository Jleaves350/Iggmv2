# Iggmv2
This program is for Sub-pixel alser line extraction. It uses a improved Gray-Gravity method (Yuehua Li, 2017).
The program is designed to run in realtime and be integrated into the unity devlopment. This program takes an input of 2 grayscale images (shown here in colour), one with a laser line, one without.
![WIN_20230319_07_52_24_Pro](https://github.com/Jleaves350/Iggmv2/assets/9415271/330777cf-e35b-499d-ba47-b6283a8dc0f0)
![WIN_20230319_07_52_45_Pro](https://github.com/Jleaves350/Iggmv2/assets/9415271/e2689943-2af8-4dd1-b93e-b1c3b8fa9bca)
It then uses the OpenCv library (wrapped to c#) to find the difference between the images essentially extracting a highly defined laser line (insert image)
This code is designed to run on an arm that can move in the x-y plane with a fixed camera and a fixed laser source. This allows me to simply the algorithm in (Yuehua Li, 2017) further.
In the original IGGM (Improved Gray-gravity method) the authors call for summing the gray intensity of pixels along an orthogonal line to a rectangular search section shown below.

![sensors-17-00814-g001-550](https://github.com/Jleaves350/Iggmv2/assets/9415271/e07d55c1-0d5d-4908-a41c-db4cc3841dd5) (Yuehua Li, 2017).

This would add signifigant complexity to my program and increase the execute time. So in order to reduce the time complexitiy I recognised that in the case of a fixed laser
beam accross a pcb there would be no signifigant radius of curvature introduced by the laser line being displaced by a component on the pcb. Therefore it is essentially a step function at points where
there is a height change (e.g the edge of the pcb and the surface it's on). Therefore my program only has to do the gray gravity summation along 1 perpandicular line, moreover the angle of this line is known due to
the fixed relationship of the camera and the laser source. This means that my all my code needs to do is normalise the gray laser line, rotate the image by the known angle, and sum the gray intensity of each pixel in 
vertical lines along the image according to:
```math
C_P(x_p,y_p)=(j,\sum_{j=1}^{M} I_n(i,j)*i/\sum_{j=1}^{M}I_n(i,j)).
```
The optimisation performed allows this code to run quicker than the IGGM in (Yuehua Li, 2017) while retaining the sub-pixel accuracy. Using the images above as inputs the code outputs sub-pixel values
for the centre point, rouding the values to integers it produces this centre line (overlapped onto the laser line).

![12](https://github.com/Jleaves350/Iggmv2/assets/9415271/0d2fbe76-cc7c-4b46-93c9-a7aa682e1a80)


Li, Y.; Zhou, J.; Huang, F.; Liu, L. Sub-Pixel Extraction of Laser Stripe Center Using an Improved Gray-Gravity Method †. Sensors 2017, 17, 814. https://doi.org/10.3390/s17040814

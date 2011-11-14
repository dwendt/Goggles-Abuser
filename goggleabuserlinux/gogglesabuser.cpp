/*************************************************
*                                                *
*  EasyBMP Cross-Platform Windows Bitmap Library * 
*                                                *
*  Author: Paul Macklin                          *
*   email: macklin01@users.sourceforge.net       *
* support: http://easybmp.sourceforge.net        *
*                                                *
*          file: EasyBMPsample.cpp               * 
*    date added: 03-31-2006                      *
* date modified: 12-01-2006                      *
*       version: 1.06                            *
*                                                *
*   License: BSD (revised/modified)              *
* Copyright: 2005-6 by the EasyBMP Project       * 
*                                                *
* description: Sample application to demonstrate *
*              some functions and capabilities   *
*                                                *
*************************************************/

#include <stdlib.h>
#include "EasyBMP.h"
using namespace std;


string numToB64(int num) {
	string b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
	num = num+131072;
	string result = "";
	for (int i=0; i<3; i++) {
		result = b64[ num%64 ] + result;
		num = (int)num/64;
	}
	
	return result;
}



int main( int argc, char* argv[] )
{
	BMP toPost;
	toPost.ReadFromFile("post.bmp");
	
//	pointData dotArray[toPost.TellWidth()][toPost.TellHeight()];
	
	int scale = 1;
	int xoffset = 0;
	int yoffset = 0;
	
	cout << "Width: " << toPost.TellWidth() << " Height: " << toPost.TellHeight() << " Pixels: " << toPost.TellWidth()*toPost.TellHeight() << endl;
	for(int x=0; x<toPost.TellWidth(); x++) {
		for(int y=0; y<toPost.TellHeight(); y++) {
			
			string pointSerialized = numToB64(x * scale + xoffset) + numToB64(y * scale + yoffset);
			
			cout << pointSerialized << endl;
			cout << (int)toPost(x,y)->Red << endl;
			
		}
	}
	
	
	return 0;
}

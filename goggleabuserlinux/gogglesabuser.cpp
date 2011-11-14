
#include <curl/curl.h>
#include <stdlib.h>
#include <sstream>
#include "EasyBMP.h"
using namespace std; //yup

template<typename T>
string to_string(const T& t) {
	stringstream s;
	s << t;
	return s.str();
}

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
	int diameter = 7;
	
	
	cout << "Width: " << toPost.TellWidth() << " Height: " << toPost.TellHeight() << " Pixels: " << toPost.TellWidth()*toPost.TellHeight() << endl;


	CURL *curl;
	CURLcode res;
	
	curl = curl_easy_init();
	if(curl) {
		
		for(int x=0; x<toPost.TellWidth(); x++) {
			for(int y=0; y<toPost.TellHeight(); y++) {
				
				string pointSerialized = numToB64(x * scale + xoffset) + numToB64(y * scale + yoffset);
				
	//			cout << pointSerialized << endl;
	//			cout << (int)toPost(x,y)->Red << endl;
				
				//yes this is bad coding
				string myURL = "http%3A%2F%2Fwww.mspaintadventures.com%2F%2F";
				string fullURL = "http://goggles.sneakygcr.net/page?callback=fuckoff&page=" + myURL + "&add=t&title=GNAA&r=" + to_string((int)toPost(x,y)->Red) + "&g=" +  to_string((int)toPost(x,y)->Green) + "&b=" + to_string((int)toPost(x,y)->Blue) + "&a=1&t=" + to_string(diameter) + "&p=" + pointSerialized + "&_=1420891562000";
				
	//			system(("curl \"" + fullURL + "\"").c_str());
				res = curl_easy_setopt(curl, CURLOPT_URL, fullURL.c_str());
				
				curl_easy_perform(curl);
				

			}
		}
	
	}

	return 0;
}

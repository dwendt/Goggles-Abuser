CC = g++

CFLAGS = -O3 -pipe -fomit-frame-pointer -funroll-all-loops -s

gogglesabuser: EasyBMP.o gogglesabuser.o
	g++ $(CFLAGS) EasyBMP.o gogglesabuser.o -o gogglesabuser

EasyBMP.o: EasyBMP.cpp
	g++ $(CFLAGS) -c EasyBMP.cpp

gogglesabuser.o: gogglesabuser.cpp
	g++ -c gogglesabuser.cpp

clean: 
	rm -f *.o
	rm gogglesabuser

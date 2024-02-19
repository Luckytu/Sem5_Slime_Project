#include "FastLED.h"

///define constants
#define NUM_LEDS 120
#define DATA_PIN1 3
#define DATA_PIN2 4

///define leds with length from constant
CRGB leds[NUM_LEDS];

///length of character array
const byte numChars = 32;

///define array to store received data
char receivedChars[numChars];   // an array to store the received data

///initialize hsv variables
int hValue = 0;
int sValue = 0;
int vValue = 0;
int length = 0;

///flags for data receiving and animation running
boolean newData = false;
boolean doAnimation = false;

///setup function to start serial communication and add leds
void setup() {
    Serial.begin(9600);
    delay(2000);
    FastLED.addLeds<WS2812B, DATA_PIN1, RGB>(leds, NUM_LEDS);
    FastLED.addLeds<WS2812B, DATA_PIN2, RGB>(leds, NUM_LEDS);
}

///loop function to read data from usb then write recieved data to serial then do led animation
void loop() {
    recvWithEndMarker();
    showNewData();
    ledAnimation();
}

///function to read data from usb
void recvWithEndMarker() {
    static byte ndx = 0;
    char endMarker = '\n';
    char rc;
    
    while (Serial.available() > 0 && newData == false) {
        rc = Serial.read();

        if (rc != endMarker) {
            receivedChars[ndx] = rc;
            ndx++;
            if (ndx >= numChars) {
                ndx = numChars - 1;
            }
        }
        else {
            receivedChars[ndx] = '\0'; // terminate the string
            ndx = 0;
            newData = true;
            hValue = atoi(receivedChars);
            sValue = atoi(&receivedChars[4]);
            vValue = atoi(&receivedChars[8]);
            length = atoi(&receivedChars[12]);
            doAnimation = true;
        }
    }
}

///function to write data to serial for debugging hsv values
void showNewData() {
    if (newData == true) {
        Serial.print(hValue);
        Serial.print(", ");
        Serial.print(sValue);
        Serial.print(", ");
        Serial.print(vValue);
        Serial.print(", ");
        Serial.println(length);
        newData = false;
    }
}

///function to light up leds with received hsv values
void ledAnimation() {
  if(doAnimation) {
    int y = NUM_LEDS - 40;
    for(int x = 0; x < NUM_LEDS; ++x){
      leds[x] = CRGB(0,0,0);
      FastLED.show();
    }
    for(int x = 0; x < NUM_LEDS; ++x){
      leds[x] = CHSV(hValue,sValue,vValue);
      leds[y] = CRGB(0,0,0);
      delay(4);
      FastLED.show();
      y++;
      if(y >= NUM_LEDS) {
        y = 0;
      }
    }
    for(int x = NUM_LEDS - 40; x < NUM_LEDS; ++x){
      leds[x] = CRGB(0,0,0);
      FastLED.show();
      delay(4);
    }
    Serial.println("finished");
    doAnimation = false;
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallTypesHelper
{
    /* 1's indicate floor tile, 0's indicate empty space
     * search adjacent tiles starting from the top and moving
     * clockwise.
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
     * Example: 
     * 0b0100 indicates a wall tile with a floor tile 
     * on the adjacent tile to the right only
     * 
     * Note:
     * 2 byte binary numbers will be considering corner tiles as well
     * (checks in clockwise order as with 1 byte binary numbers)
     */

    public static HashSet<int> wallHorizontalSingle = new HashSet<int>
    {
        0b1111
    };

    public static HashSet<int> wallHorizontalLeft = new HashSet<int> 
    {
        0b1011
    };

    public static HashSet<int> wallHorizontalMiddle = new HashSet<int> 
    {
        0b0000,
        0b0010,
        0b1000,
        0b1010
    };
    public static HashSet<int> wallHorizontalRight = new HashSet<int> 
    { 
        0b1110
    };

    public static HashSet<int> wallVerticalTop = new HashSet<int> 
    { 
        0b1101
    };

    public static HashSet<int> wallVerticalMiddle = new HashSet<int> 
    { 
        0b0001,
        0b0100,
        0b0101
    };

    public static HashSet<int> wallVerticalBottom = new HashSet<int> 
    { 
        0b0111
    };
    
    public static HashSet<int> wallInnerCornerTopLeft = new HashSet<int> 
    { 
        0b1001
    };

    public static HashSet<int> wallInnerCornerTopRight = new HashSet<int> 
    { 
        0b1100
    };

    public static HashSet<int> wallInnerCornerBottomRight = new HashSet<int> 
    { 
        0b0110    
    };
    
    public static HashSet<int> wallInnerCornerBottomLeft = new HashSet<int> 
    { 
        0b0011
    };

    public static HashSet<int> wallOuterCornerTopLeft = new HashSet<int>
    {
        0b00010000
    };

    public static HashSet<int> wallOuterCornerTopRight = new HashSet<int>
    {
        0b00000100
    };

    public static HashSet<int> wallOuterCornerBottomRight = new HashSet<int>
    {
        0b00000001
    };

    public static HashSet<int> wallOuterCornerBottomLeft = new HashSet<int>
    {
        0b01000000
    };

    public static HashSet<int> wallTeeUp = new HashSet<int>
    {
        0b01000001,
        0b01011000,
        0b00011001,
        0b01011100,
        0b00001101,
        0b00011101,
        0b01001100
    };

    public static HashSet<int> wallTeeRight = new HashSet<int>
    {
        0b01010000,
        0b01000111,
        0b01000011,
        0b00010110,
        0b00000010
    };

    public static HashSet<int> wallTeeDown = new HashSet<int>
    {
        0b00010100,
        0b10000101,
        0b11010001,
        0b11010101,
        0b11000101,
        0b10000100,
        0b11010000,
        0b11000100,
        0b10010001
    };

    public static HashSet<int> wallTeeLeft = new HashSet<int>
    {
        0b00000101,
        0b01110001,
        0b00110100,
        0b01110100,
        0b00110001,
        0b01100100
    };

    public static HashSet<int> wallCross = new HashSet<int>
    {
        0b01000100,
        0b00010001,
        0b01010100,
        0b01010001,
        0b01000101,
        0b00010101,
        0b01010101
    };

    /* public static HashSet<int> wallTop = new HashSet<int>
     {
         0b1111,
         0b0110,
         0b0011,
         0b0010,
         0b1010,
         0b1100,
         0b1110,
         0b1011,
         0b0111
     };

     public static HashSet<int> wallSideLeft = new HashSet<int>
     {
         0b0100
     };

     public static HashSet<int> wallSideRight = new HashSet<int>
     {
         0b0001
     };

     public static HashSet<int> wallBottom = new HashSet<int>
     {
         0b1000
     };

     public static HashSet<int> wallInnerCornerDownLeft = new HashSet<int>
     {
         0b11110001, *
         0b11100000, *
         0b11110000, *
         0b11100001, *
         0b10100000, *
         0b01010001, *
         0b11010001, *
         0b01100001, *
         0b11010000, *
         0b01110001, *
         0b00010001, *
         0b10110001, *
         0b10100001, *
         0b10010000, *
         0b00110001, *
         0b10110000, *
         0b00100001, *
         0b10010001 *
     };

     public static HashSet<int> wallInnerCornerDownRight = new HashSet<int>
     {
         0b11000111, *
         0b11000011, *
         0b10000011, *
         0b10000111, *
         0b10000010, *
         0b01000101, *
         0b11000101, *
         0b01000011, *
         0b10000101, *
         0b01000111, *
         0b01000100, *
         0b11000110, *
         0b11000010, *
         0b10000100, *
         0b01000110, 
         0b10000110,
         0b11000100,
         0b01000010

     };

     public static HashSet<int> wallDiagonalCornerDownLeft = new HashSet<int>
     {
         0b01000000
     };

     public static HashSet<int> wallDiagonalCornerDownRight = new HashSet<int>
     {
         0b00000001
     };

     public static HashSet<int> wallDiagonalCornerUpLeft = new HashSet<int>
     {
         0b00010000,
         0b01010000,
     };

     public static HashSet<int> wallDiagonalCornerUpRight = new HashSet<int>
     {
         0b00000100,
         0b00000101
     };

     public static HashSet<int> wallFull = new HashSet<int>
     {
         0b1101,
         0b0101,
         0b1101,
         0b1001

     };

     public static HashSet<int> wallFullEightDirections = new HashSet<int>
     {
         0b00010100, *
         0b11100100, *
         0b10010011, *
         0b01110100, *
         0b00010111, *
         0b00010110, *
         0b00110100, *
         0b00010101, *
         0b01010100, *
         0b00010010, *
         0b00100100, *
         0b00010011, *
         0b01100100, *
         0b10010111, *
         0b11110100, *
         0b10010110, *
         0b10110100, *
         0b11100101, *
         0b11010011, *
         0b11110101, *
         0b11010111, *
         0b01110101, *
         0b01010111, * 
         0b01100101, *
         0b01010011, *
         0b01010010, * 
         0b00100101,
         0b00110101,
         0b01010110,
         0b11010101,
         0b11010100,
         0b10010101
     };

     public static HashSet<int> wallBottomEightDirections = new HashSet<int>
     {
         0b01000001
     };*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class State : Singleton<State>
{


    public struct Level
    {
        public int current;
        
        /*
        1 - CHOOSE GENOMIC SEQUENCE
        2 - SHOW END OF LINE MARKER ADDED
        ... etc
        */
    }

    public struct Counts
    {
        public int nucleotides;
    }


    
    // single instances declared here
    // multiple instances declared within other classes, although still available globally

    public static Level level;
    public static Counts counts;



    public static void Load()
    {
        level.current = 1;
        counts.nucleotides = 0;
    }        

}

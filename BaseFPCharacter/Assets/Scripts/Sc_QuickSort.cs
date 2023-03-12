using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Needed to make a quick sort algorithem to organize the list of AI Attacks in the AIDirector script based on their decision value.
 So I grabed the base script from geeks to geeks and then changed the variables types so it would spawn the correct objects 
 */
public class Sc_QuickSort : MonoBehaviour
{
    // A utility function to swap two elements
    static void Swap(List<GameObject> mainArr, int i, int j)
    {
        GameObject temp = mainArr[i];
        mainArr[i] = mainArr[j];
        mainArr[j] = temp;
    }

    /* This function takes last element as pivot, places
         the pivot element at its correct position in sorted
         array, and places all smaller (smaller than pivot)
         to left of pivot and all greater elements to right
         of pivot */
    static int Partition(List<GameObject> arrayObjects, int lowEnd, int highEnd)
    {

        // pivot
        int pivot = arrayObjects[highEnd].GetComponent<Sc_AIStateManager>().ReturnDecisionValue();

        // Index of smaller element and
        // indicates the right position
        // of pivot found so far
        int i = (lowEnd - 1);

        for (int j = lowEnd; j <= highEnd - 1; j++)
        {

            // If current element is smaller
            // than the pivot
            if (arrayObjects[j].GetComponent<Sc_AIStateManager>().ReturnDecisionValue() < pivot)
            {

                // Increment index of
                // smaller element
                i++;
                Swap(arrayObjects, i, j);
            }
        }
        Swap(arrayObjects, i + 1, highEnd);
        return (i + 1);
    }

    /* The main function that implements QuickSort
                arr[] --> Array to be sorted,
                low --> Starting index,
                highEnd --> Ending index
       */
    static void QuickSort(List<GameObject> arrayObjects, int lowEnd, int highEnd)
    {
        if (lowEnd < highEnd)
        {

            // pi is partitioning index, arr[p]
            // is now at right place
            int pi = Partition(arrayObjects, lowEnd, highEnd);

            // Separately sort elements before
            // partition and after partition
            QuickSort(arrayObjects, lowEnd, pi - 1);
            QuickSort(arrayObjects, pi + 1, highEnd);
        }
    }

    // Function to print an array
    static void PrintArray(int[] arr, int size)
    {
        for (int i = 0; i < size; i++)
            Console.Write(arr[i] + " ");

        Console.WriteLine();
    }

    // Driver Code
    public void Main(List<GameObject> arrayObjects)
    {
        int length = arrayObjects.Count;

        QuickSort(arrayObjects, 0, length - 1);
        Console.Write("Sorted array: ");
        //PrintArray(arr, n);
    }
}

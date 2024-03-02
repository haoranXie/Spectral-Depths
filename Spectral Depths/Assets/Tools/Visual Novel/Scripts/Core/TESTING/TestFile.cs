using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestFiles : MonoBehaviour
    {
        [SerializeField] private TextAsset fileName;//fileName is name of file you want to read(Can be changed in inspector)
                                                    // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            List<string> lines = FileManager.ReadTextAsset(fileName, false);//Sets the list, "lines" as the output of method ReadTextFile or ReadTextAsset from FileManager class the true or false indicates whether to include space between lines or not(true = add lines, false = don't add lines)

            foreach (string line in lines)
                Debug.Log(line);

            yield return null;
        }
    }
}
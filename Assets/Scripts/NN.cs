using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN : MonoBehaviour
{
    // Definiera arkitekturen för det neurala nätverket med antalet noder i varje skikt
    int[] networkShape = { 9, 16, 8, 2 };
    public Layer[] layers;


    // Definiera en klass för varje lager i det neurala nätverket
    public class Layer
    {
        public float[,] weightsArray; // Vikter för kopplingar mellan noder
        public float[] biasesArray; // Biases för varje nod
        public float[] nodeArray; // Utvärden från noderna


        private int nodes;
        private int inputs;

        // Konstruktor för att initialisera lagret med det angivna antalet inputs och noder
        public Layer(int inputs, int nodes)
        {
            this.nodes = nodes;
            this.inputs = inputs;


            weightsArray = new float[nodes, inputs];
            biasesArray = new float[nodes];
            nodeArray = new float[nodes];

            XavierInitialization();
        }

        public void XavierInitialization()
        {
            //Xavier initialization för att ge en head start
            for (int i = 0; i < weightsArray.GetLength(0); i++)
            {
                for (int j = 0; j < weightsArray.GetLength(1); j++)
                {
                    weightsArray[i, j] = Random.Range(-1f, 1f) * Mathf.Sqrt(2f / inputs);
                }
                biasesArray[i] = 0f;
            }
        }

        // Utför forward propagation för lagret med de angivna inputvärdena
        public void Forward(float[] inputsArray)
        {
            nodeArray = new float[nodes];

           
            for (int node = 0; node < nodes; node++)
            {
                //summan av vikterna gånger inputs
                for(int input = 0; input < inputs; input++)
                {
                    nodeArray[node] += weightsArray[node, input] * inputsArray[input];
                }

                //lägg till bias
                nodeArray[node] += biasesArray[node];
            }
        }

        // Tillämpa aktiveringsfunktionen (ReLU i detta fall) på noderna
        public void Activation()
        {
            for(int i = 0; i < nodes; i++)
            {
                if (nodeArray[i] < 0)
                {
                    nodeArray[i] = 0;
                }
            }
        }

        // Tillämpa aktiveringsfunktionen (Tanh i detta fall) på noderna
        public void TanhActivation()
        {
            for (int i = 0; i < nodes; i++)
            {
                nodeArray[i] = (float)System.Math.Tanh(nodeArray[i]);
            }
        }

        // Mutera lagrets vikter och biases med en viss sannolikhet och mängd
        public void MutateLayer(float mutationChance, float mutationAmount)
        {
            for (int i = 0; i < nodes; i++)
            {
                for (int j = 0; j < inputs; j++)
                {
                    if (Random.value < mutationChance)
                    {
                        //weightsArray[i, j] += Random.Range(-1.0f, 1.0f) * mutationAmount;
                        weightsArray[i, j] += Random.Range(-0.5f, 0.5f) * mutationAmount;
                    }
                }

                if (Random.value < mutationChance)
                {
                    //biasesArray[i] += Random.Range(-1.0f, 1.0f) * mutationAmount;
                    biasesArray[i] += Random.Range(-0.5f, 0.5f) * mutationAmount;
                }
            }
        }
        

    }

    // Mutera alla lager i det neurala nätverket
    public void MutateNetwork(float mutationChance, float mutationAmount)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].MutateLayer(mutationChance, mutationAmount);
        }
    }

    // Initialisera lagren i det neurala nätverket
    public void Awake()
        {
        layers = new Layer[networkShape.Length - 1];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(networkShape[i], networkShape[i + 1]);
        }
    }

    public void XavierReset()
    {
        foreach (Layer layer in layers)
        {
            layer.XavierInitialization();
        }
    }
    // Skapa en kopia av lagren i det neurala nätverket
    public Layer[] CopyLayers()
    {
            Layer[] tmpLayers = new Layer[networkShape.Length - 1];
            for (int i = 0; i < layers.Length; i++)
            {
                tmpLayers[i] = new Layer(networkShape[i], networkShape[i + 1]);
                System.Array.Copy(layers[i].weightsArray, tmpLayers[i].weightsArray, layers[i].weightsArray.GetLength(0) * layers[i].weightsArray.GetLength(1));
                System.Array.Copy(layers[i].biasesArray, tmpLayers[i].biasesArray, layers[i].biasesArray.GetLength(0));
            }
            return (tmpLayers);
    }


    //Kör igenom det neurala nätverket för att få output värden
    public float[] Brain(float[] inputs)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (i == 0)
            {
                layers[i].Forward(inputs);
                layers[i].Activation();
            }

            //Output layer. Begränsa resultatet med tanh för att få mjukare och mer nyanserade värden
            else if (i == layers.Length - 1)
            {
                layers[i].Forward(layers[i - 1].nodeArray);
                //layers[i].TanhActivation();

            }
            else
            {
                layers[i].Forward(layers[i - 1].nodeArray);
                layers[i].Activation();
            }
        }

        return (layers[layers.Length - 1].nodeArray);
    }
}

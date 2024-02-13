using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagramManager : MonoBehaviour
{

    [SerializeField] private NodeRenderer nodeBase;
    [SerializeField] private PathRenderer pathBase;
    public List<NodeRenderer> nodes;
    public List<PathRenderer> paths;

    void Update()
    {
        while (PJ.requests.Count > 0)
        {
            var (req, index, values) = PJ.requests[0];
            string action = values.action;
            PJ.requests.RemoveAt(0);

            if (req.Equals("ADD"))
            {
                // make space for new node
                for (int i = index; i < PJ.X.Count; i++)
                {
                    nodes[i].SetIndex(i + 1);
                    paths[i].SetIndex(i + 1);
                }

                PJ.X.Insert(index, values.x);
                PJ.Y.Insert(index, values.y);
                PJ.HEADING.Insert(index, values.heading);
                PJ.ACTION.Insert(index, values.action);
                PJ.ARM.Insert(index, values.arm);
                PJ.DELAY.Insert(index, values.delay);

                nodes.Insert(index, Instantiate(nodeBase, transform).SetIndex(index));
                paths.Insert(index, Instantiate(pathBase, transform).SetIndex(index));
            }

            if (req.Equals("CLEAR") && (0 <= index && index < PJ.X.Count))
            {
                for (int i = PJ.X.Count - 1; i >= index; i--)
                {
                    nodes[i].Kill();
                    paths[i].Kill();
                    nodes.RemoveAt(i); 
                    paths.RemoveAt(i);

                    PJ.X.RemoveAt(i);
                    PJ.Y.RemoveAt(i);
                    PJ.HEADING.RemoveAt(i);
                    PJ.ARM.RemoveAt(i);
                    PJ.ACTION.RemoveAt(i);
                    PJ.DELAY.RemoveAt(i);
                }
            }

            if (req.Equals("REPLACE"))
            {
                PJ.X[index] = values.x;
                PJ.Y[index] = values.y;
                PJ.HEADING[index] = values.heading;
                PJ.ARM[index] = values.arm;
                PJ.ACTION[index] = values.action;
                PJ.DELAY[index] = values.delay;
            }

        }

    }

}

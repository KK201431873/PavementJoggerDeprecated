using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{

    [SerializeField] private RenderedNode nodeBase;
    public List<RenderedNode> nodes;

    void Update()
    {
        while (PJ.requests.Count > 0)
        {
            var (req, index, values) = PJ.requests[^1];
            PJ.requests.RemoveAt(PJ.requests.Count - 1);

            if (req.Equals("ADD"))
            {
                // make space for new node
                for (int i = index; i < PJ.X.Count; i++)
                {
                    nodes[i].SetIndex(i + 1);
                }
                nodes.Insert(index, Instantiate(nodeBase).SetIndex(index));

                PJ.X.Insert(index, values.x);
                PJ.Y.Insert(index, values.y);
                PJ.HEADING.Insert(index, values.heading);
                PJ.ACTION.Insert(index, values.action);
                PJ.ARM.Insert(index, values.arm);
                PJ.DELAY.Insert(index, values.delay);
            }

            if (req.Equals("CLEAR"))
            {
                for (int i = PJ.X.Count - 1; i >= index; i++)
                {
                    PJ.X.RemoveAt(i);
                    PJ.Y.RemoveAt(i);
                    PJ.HEADING.RemoveAt(i);
                    PJ.ARM.RemoveAt(i);
                    PJ.ACTION.RemoveAt(i);
                    PJ.DELAY.RemoveAt(i);
                    Destroy(nodes[i]);
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

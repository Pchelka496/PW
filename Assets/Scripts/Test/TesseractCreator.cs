using UnityEngine;

namespace Test
{
    public class TesseractCreator : MonoBehaviour
    {
        [SerializeField, Min(2)] private int cubesPerAxis = 3; // Количество кубов на ось
        [SerializeField] private float cubeSize = 1f; // Размер куба
        [SerializeField] private float jointBreakForce = 1000f; // Сила разрушения джоинта

        [ContextMenu("Create Tesseract")]
        private void CreateTesseract()
        {
            ClearExistingCubes(); // Удаляем предыдущие кубы, если есть
            CreateCubesWithJoints();
        }

        private void CreateCubesWithJoints()
        {
            var startPosition = transform.position - Vector3.one * (cubesPerAxis - 1) * cubeSize * 0.5f;

            GameObject[,,] cubes = new GameObject[cubesPerAxis, cubesPerAxis, cubesPerAxis];

            for (int x = 0; x < cubesPerAxis; x++)
            {
                for (int y = 0; y < cubesPerAxis; y++)
                {
                    for (int z = 0; z < cubesPerAxis; z++)
                    {
                        Vector3 position = startPosition + new Vector3(x, y, z) * cubeSize;
                        cubes[x, y, z] = CreateCube(position);

                        // Присоединяем текущий куб ко всем соседним
                        if (x > 0) ConnectCubes(cubes[x, y, z], cubes[x - 1, y, z]);
                        if (y > 0) ConnectCubes(cubes[x, y, z], cubes[x, y - 1, z]);
                        if (z > 0) ConnectCubes(cubes[x, y, z], cubes[x, y, z - 1]);
                    }
                }
            }
        }

        private GameObject CreateCube(Vector3 position)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * cubeSize;

            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.mass = 1f; // Устанавливаем массу для физики

            return cube;
        }

        private void ConnectCubes(GameObject cubeA, GameObject cubeB)
        {
            FixedJoint joint = cubeA.AddComponent<FixedJoint>();
            joint.connectedBody = cubeB.GetComponent<Rigidbody>();
            joint.breakForce = jointBreakForce; // Устанавливаем силу разрушения
        }

        private void ClearExistingCubes()
        {
            // Удаляем все дочерние объекты
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}

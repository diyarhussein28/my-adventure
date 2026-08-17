using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SeasOfLegends.World
{
    /// <summary>
    /// Lumenwake Reef's runtime world assembly. It uses the project's free licensed Poly Haven
    /// source models for the visible coastal landmarks and reserves generated meshes for modular
    /// reef-town architecture rather than gameplay primitives.
    /// </summary>
    public static class CinematicIslandBuilder
    {
        private const string BeachDiffuse = "Environment/PBR/Beach/aerial_beach_01_diff_1k";
        private const string GroundDiffuse = "Art/Environment/tropical_island_ground";
        private const string WaterDiffuse = "Art/Environment/ocean_water";
        private const string RockModel = "Lumenwake/Environment/coast_rocks_05/coast_rocks_05_1k";
        private const string PierModel = "Lumenwake/Environment/modular_wooden_pier/modular_wooden_pier_1k";
        private const string BarrelModel = "Lumenwake/Environment/wine_barrel_01/wine_barrel_01_1k";
        private const string GateModel = "Lumenwake/Environment/large_castle_door/large_castle_door_1k";
        private const string ShipModel = "Lumenwake/Environment/ship_pinnace/ship_pinnace_1k";

        public static void Build(Transform root)
        {
            ClearOldWorld(root);
            ConfigureLumenwakeLighting();
            CreateLagoon(root);
            Terrain terrain = CreateReefTerrain(root);
            CreateFreeAssetLandmarks(root, terrain);
            CreateReefTown(root, terrain);
            CreateBreathingEye(root, terrain);
        }

        private static void ClearOldWorld(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Transform child = root.GetChild(i);
                if (child.name.StartsWith("Lumenwake") || child.name.StartsWith("Starter Island") || child.name.StartsWith("Open Ocean"))
                    Object.Destroy(child.gameObject);
            }
        }

        private static void ConfigureLumenwakeLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.19f, 0.39f, 0.59f);
            RenderSettings.ambientEquatorColor = new Color(0.42f, 0.35f, 0.26f);
            RenderSettings.ambientGroundColor = new Color(0.06f, 0.09f, 0.11f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.25f, 0.48f, 0.63f);
            RenderSettings.fogDensity = 0.0027f;
            RenderSettings.reflectionIntensity = 0.78f;

            GameObject sunObject = new GameObject("Lumenwake Golden-Hour Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.68f, 0.43f);
            sun.intensity = 1.15f;
            sun.shadowStrength = 0.9f;
            sun.shadowBias = 0.04f;
            sunObject.transform.rotation = Quaternion.Euler(38f, -32f, 0f);
        }

        private static void CreateLagoon(Transform root)
        {
            GameObject ocean = CreatePanel("Lumenwake Lagoon", new Vector3(0f, -0.24f, 0f), new Vector2(420f, 420f), CreateMaterial(new Color(0.025f, 0.25f, 0.37f), 0.8f, 0f, WaterDiffuse));
            ocean.transform.SetParent(root);
            ocean.AddComponent<OceanSurfaceAnimator>();
        }

        private static Terrain CreateReefTerrain(Transform root)
        {
            const int resolution = 257;
            TerrainData data = new TerrainData
            {
                heightmapResolution = resolution,
                alphamapResolution = 256,
                size = new Vector3(170f, 24f, 150f)
            };

            float[,] heights = new float[resolution, resolution];
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float u = x / (float)(resolution - 1);
                    float v = z / (float)(resolution - 1);
                    float coastMask = Mathf.Clamp01(1f - Mathf.Pow(Mathf.Abs(u - 0.51f) * 1.12f, 1.7f) - Mathf.Pow(Mathf.Abs(v - 0.53f) * 1.42f, 1.35f));
                    float terrace = Mathf.SmoothStep(0f, 1f, coastMask) * 0.18f;
                    float ridge = Mathf.PerlinNoise(u * 5.2f + 16f, v * 4.1f + 3f) * 0.058f * coastMask;
                    float reefShelf = Mathf.PerlinNoise(u * 19f, v * 17f) * 0.012f * Mathf.SmoothStep(0.72f, 0.1f, coastMask);
                    heights[z, x] = Mathf.Clamp(0.01f + terrace + ridge + reefShelf, 0.01f, 0.31f);
                }
            }
            data.SetHeights(0, 0, heights);

            TerrainLayer beach = new TerrainLayer { diffuseTexture = Resources.Load<Texture2D>(BeachDiffuse), tileSize = new Vector2(8f, 8f) };
            TerrainLayer reefstone = new TerrainLayer { diffuseTexture = Resources.Load<Texture2D>(GroundDiffuse), tileSize = new Vector2(11f, 11f) };
            data.terrainLayers = new[] { beach, reefstone };
            float[,,] alpha = new float[256, 256, 2];
            for (int z = 0; z < 256; z++)
            {
                for (int x = 0; x < 256; x++)
                {
                    float h = heights[Mathf.RoundToInt(z / 255f * (resolution - 1)), Mathf.RoundToInt(x / 255f * (resolution - 1))];
                    float reef = Mathf.SmoothStep(0.07f, 0.18f, h);
                    alpha[z, x, 0] = 1f - reef;
                    alpha[z, x, 1] = reef;
                }
            }
            data.SetAlphamaps(0, 0, alpha);
            GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
            terrainObject.name = "Lumenwake Reef Terraces";
            terrainObject.transform.SetParent(root);
            terrainObject.transform.position = new Vector3(-85f, -0.03f, -75f);
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            terrain.drawInstanced = true;
            terrain.heightmapPixelError = 2.5f;
            terrain.materialTemplate = null;
            return terrain;
        }

        private static void CreateFreeAssetLandmarks(Transform root, Terrain terrain)
        {
            Vector3[] rocks =
            {
                new Vector3(-52f, 0f, -38f), new Vector3(-41f, 0f, 30f), new Vector3(42f, 0f, -26f),
                new Vector3(57f, 0f, 19f), new Vector3(-12f, 0f, 48f), new Vector3(18f, 0f, 38f)
            };
            for (int i = 0; i < rocks.Length; i++)
            {
                rocks[i].y = GroundHeight(terrain, rocks[i]) - 0.75f;
                InstantiateAsset(RockModel, "Lumenwake Coastal Rock", root, rocks[i], Quaternion.Euler(0f, i * 49f, 0f), Vector3.one * (1.5f + (i % 3) * 0.4f));
            }

            Vector3 pierPosition = new Vector3(-55f, GroundHeight(terrain, new Vector3(-55f, 0f, -11f)) - 0.15f, -11f);
            InstantiateAsset(PierModel, "Fishermen's Tide Pier", root, pierPosition, Quaternion.Euler(0f, -18f, 0f), Vector3.one * 1.55f);

            GameObject ship = InstantiateAsset(ShipModel, "The Saltworn Pinnace", root, new Vector3(-73f, 0.22f, -25f), Quaternion.Euler(0f, 148f, 1f), Vector3.one * 0.7f);
            if (ship != null) ship.AddComponent<OceanSurfaceAnimator>();

            Vector3[] barrelPositions = { new Vector3(-42f, 0f, -5f), new Vector3(-40f, 0f, -6.5f), new Vector3(13f, 0f, 13f), new Vector3(14.4f, 0f, 12.4f) };
            for (int i = 0; i < barrelPositions.Length; i++)
            {
                barrelPositions[i].y = GroundHeight(terrain, barrelPositions[i]);
                InstantiateAsset(BarrelModel, "Harbor Barrel", root, barrelPositions[i], Quaternion.Euler(0f, i * 67f, 0f), Vector3.one * 0.85f);
            }
        }

        private static void CreateReefTown(Transform root, Terrain terrain)
        {
            CreateReefHouse(root, terrain, new Vector3(-18f, 0f, 15f), 0f, 1.25f, "Tidewarden's House");
            CreateReefHouse(root, terrain, new Vector3(-3f, 0f, 21f), 24f, 0.95f, "Netmaker's House");
            CreateReefHouse(root, terrain, new Vector3(15f, 0f, 18f), -19f, 1.15f, "Pearl Market House");
            CreateReefHouse(root, terrain, new Vector3(24f, 0f, 5f), -49f, 0.86f, "Watch House");
            CreateLanternLine(root, terrain, new Vector3(-30f, 0f, 5f), new Vector3(25f, 0f, 10f), 9);
        }

        private static void CreateReefHouse(Transform root, Terrain terrain, Vector3 location, float yaw, float scale, string name)
        {
            location.y = GroundHeight(terrain, location);
            GameObject house = new GameObject(name);
            house.transform.SetParent(root);
            house.transform.position = location;
            house.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            house.transform.localScale = Vector3.one * scale;

            Material stone = CreateMaterial(new Color(0.46f, 0.38f, 0.29f), 0.35f, 0f, BeachDiffuse);
            Material paintedWood = CreateMaterial(new Color(0.06f, 0.20f, 0.24f), 0.42f, 0f, GroundDiffuse);
            Material roof = CreateMaterial(new Color(0.08f, 0.13f, 0.23f), 0.68f, 0f, null);
            CreateBox(house.transform, "Reefstone Foundation", new Vector3(0f, 0.75f, 0f), new Vector3(7.2f, 1.5f, 5.3f), stone);
            CreateBox(house.transform, "Indigo Timber Wall", new Vector3(0f, 2.55f, 0f), new Vector3(6.5f, 2.1f, 4.6f), paintedWood);
            CreateWedgeRoof(house.transform, new Vector3(0f, 4.35f, 0f), new Vector3(7.2f, 2.3f, 5.6f), roof);
            GameObject door = InstantiateAsset(GateModel, "Reefhouse Door", house.transform, location, Quaternion.identity, Vector3.one * 0.42f);
            if (door != null)
            {
                door.transform.localPosition = new Vector3(0f, 1.75f, 2.34f);
                door.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            CreateLantern(house.transform, new Vector3(-2.2f, 3.3f, 2.55f));
        }

        private static void CreateBreathingEye(Transform root, Terrain terrain)
        {
            Vector3 eye = new Vector3(45f, 0f, 24f);
            eye.y = GroundHeight(terrain, eye) + 0.04f;
            GameObject arena = new GameObject("Lumenwake Breathing Eye Arena");
            arena.transform.SetParent(root);
            arena.transform.position = eye;

            Material reefstone = CreateMaterial(new Color(0.34f, 0.39f, 0.38f), 0.26f, 0f, GroundDiffuse);
            Material brass = CreateMaterial(new Color(0.44f, 0.29f, 0.09f), 0.62f, 0.72f, null);
            GameObject rim = CreateRing("Breathing Eye Reefstone Rim", 17f, 1.2f, reefstone);
            rim.transform.SetParent(arena.transform, false);
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                Vector3 p = new Vector3(Mathf.Cos(a) * 14.2f, 1.2f, Mathf.Sin(a) * 14.2f);
                CreateBox(arena.transform, "Tide Gate Brace", p, new Vector3(1.1f, 3.0f, 1.1f), brass);
            }
            GameObject gate = InstantiateAsset(GateModel, "Breathing Eye Tide Gate", arena.transform, eye, Quaternion.Euler(0f, 180f, 0f), Vector3.one * 1.35f);
            if (gate != null)
            {
                gate.transform.localPosition = new Vector3(0f, 2.2f, 15.5f);
                gate.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            GameObject waterCore = CreatePanel("Breathing Eye Water Core", new Vector3(0f, 0.18f, 0f), new Vector2(24f, 24f), CreateMaterial(new Color(0.02f, 0.30f, 0.42f), 0.92f, 0f, WaterDiffuse));
            waterCore.transform.SetParent(arena.transform, false);
            waterCore.AddComponent<OceanSurfaceAnimator>();
        }

        private static void CreateLanternLine(Transform root, Terrain terrain, Vector3 from, Vector3 to, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 p = Vector3.Lerp(from, to, i / (float)(count - 1));
                p.y = GroundHeight(terrain, p);
                GameObject post = CreateBox(root, "Reef Lantern Post", p + Vector3.up * 1.5f, new Vector3(0.12f, 3f, 0.12f), CreateMaterial(new Color(0.08f, 0.05f, 0.03f), 0.3f, 0f, null));
                CreateLantern(post.transform, Vector3.up * 1.4f);
            }
        }

        private static void CreateLantern(Transform parent, Vector3 localPosition)
        {
            GameObject lantern = new GameObject("Lumenwake Lantern");
            lantern.transform.SetParent(parent, false);
            lantern.transform.localPosition = localPosition;
            Light light = lantern.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.48f, 0.18f);
            light.range = 6.5f;
            light.intensity = 1.7f;
        }

        private static GameObject InstantiateAsset(string resourcePath, string name, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab == null)
            {
                Debug.LogWarning($"Lumenwake asset not found: Resources/{resourcePath}");
                return null;
            }
            GameObject instance = Object.Instantiate(prefab, parent);
            instance.name = name;
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.localScale = scale;
            return instance;
        }

        private static float GroundHeight(Terrain terrain, Vector3 position) => terrain.SampleHeight(position) + terrain.transform.position.y;

        private static GameObject CreatePanel(string name, Vector3 position, Vector2 size, Material material)
        {
            GameObject panel = new GameObject(name);
            MeshFilter filter = panel.AddComponent<MeshFilter>();
            MeshRenderer renderer = panel.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh { name = name + " Mesh" };
            float x = size.x * 0.5f;
            float z = size.y * 0.5f;
            mesh.vertices = new[] { new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z) };
            mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
            mesh.uv = new[] { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };
            mesh.RecalculateNormals();
            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
            panel.transform.position = position;
            return panel;
        }

        private static GameObject CreateBox(Transform parent, string name, Vector3 localPosition, Vector3 dimensions, Material material)
        {
            GameObject part = new GameObject(name);
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            MeshFilter filter = part.AddComponent<MeshFilter>();
            MeshRenderer renderer = part.AddComponent<MeshRenderer>();
            filter.sharedMesh = CreateBoxMesh(name + " Mesh", dimensions);
            renderer.sharedMaterial = material;
            return part;
        }

        private static GameObject CreateRing(string name, float radius, float height, Material material)
        {
            const int segments = 48;
            GameObject ring = new GameObject(name);
            MeshFilter filter = ring.AddComponent<MeshFilter>();
            MeshRenderer renderer = ring.AddComponent<MeshRenderer>();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            float inner = radius - 1.5f;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                Vector3 outer = new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
                Vector3 innerPoint = new Vector3(Mathf.Cos(angle) * inner, height, Mathf.Sin(angle) * inner);
                vertices.Add(outer); vertices.Add(innerPoint);
                if (i < segments)
                {
                    int a = i * 2;
                    triangles.Add(a); triangles.Add(a + 2); triangles.Add(a + 1);
                    triangles.Add(a + 1); triangles.Add(a + 2); triangles.Add(a + 3);
                }
            }
            Mesh mesh = new Mesh { name = name + " Mesh" };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
            return ring;
        }

        private static void CreateWedgeRoof(Transform parent, Vector3 localPosition, Vector3 dimensions, Material material)
        {
            GameObject roof = new GameObject("Blue Tile Roof");
            roof.transform.SetParent(parent, false);
            roof.transform.localPosition = localPosition;
            MeshFilter filter = roof.AddComponent<MeshFilter>();
            MeshRenderer renderer = roof.AddComponent<MeshRenderer>();
            float x = dimensions.x * 0.5f;
            float z = dimensions.z * 0.5f;
            float h = dimensions.y;
            Vector3[] vertices = { new Vector3(-x, 0f, -z), new Vector3(x, 0f, -z), new Vector3(x, 0f, z), new Vector3(-x, 0f, z), new Vector3(0f, h, -z), new Vector3(0f, h, z) };
            int[] triangles = { 0, 4, 1, 0, 3, 5, 0, 5, 4, 1, 4, 5, 1, 5, 2, 3, 2, 5 };
            Mesh mesh = new Mesh { name = "Blue Tile Roof Mesh" };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
        }

        private static Mesh CreateBoxMesh(string name, Vector3 size)
        {
            Vector3 h = size * 0.5f;
            Vector3[] v = { new Vector3(-h.x,-h.y,-h.z), new Vector3(h.x,-h.y,-h.z), new Vector3(h.x,h.y,-h.z), new Vector3(-h.x,h.y,-h.z), new Vector3(-h.x,-h.y,h.z), new Vector3(h.x,-h.y,h.z), new Vector3(h.x,h.y,h.z), new Vector3(-h.x,h.y,h.z) };
            int[] t = { 0,2,1,0,3,2, 5,6,4,4,6,7, 4,7,0,0,7,3, 1,2,6,1,6,5, 3,7,6,3,6,2, 4,0,1,4,1,5 };
            Mesh mesh = new Mesh { name = name };
            mesh.vertices = v;
            mesh.triangles = t;
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Material CreateMaterial(Color color, float smoothness, float metallic, string texturePath)
        {
            bool srp = GraphicsSettings.currentRenderPipeline != null;
            Shader shader = srp ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.color = color;
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            Texture2D texture = string.IsNullOrWhiteSpace(texturePath) ? null : Resources.Load<Texture2D>(texturePath);
            if (texture != null)
            {
                material.mainTexture = texture;
                material.mainTextureScale = new Vector2(4f, 4f);
            }
            return material;
        }
    }
}

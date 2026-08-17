using UnityEngine;
using UnityEngine.Rendering;

namespace SeasOfLegends.World
{
    /// <summary>
    /// Builds the starter exploration space from terrain, PBR source textures, hand-placed tropical
    /// landmarks, and a lightweight animated ocean. It deliberately keeps all gameplay colliders
    /// simple while replacing the former circular-island blockout with an authored-looking space.
    /// </summary>
    public static class CinematicIslandBuilder
    {
        private const string BeachDiffuse = "Environment/PBR/Beach/aerial_beach_01_diff_1k";
        private const string GrassDiffuse = "Art/Environment/tropical_island_ground";
        private const string WaterDiffuse = "Art/Environment/ocean_water";

        public static void Build(Transform root)
        {
            ConfigureLighting();
            CreateOcean(root);
            Terrain terrain = CreateTerrain(root);
            CreateLandmarks(root, terrain);
        }

        private static void ConfigureLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.34f, 0.53f, 0.68f);
            RenderSettings.ambientEquatorColor = new Color(0.52f, 0.42f, 0.29f);
            RenderSettings.ambientGroundColor = new Color(0.12f, 0.16f, 0.13f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.50f, 0.67f, 0.76f);
            RenderSettings.fogDensity = 0.004f;

            GameObject lightObject = new GameObject("Golden Hour Sun");
            Light sun = lightObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.78f, 0.56f);
            sun.intensity = 1.32f;
            sun.shadowStrength = 0.78f;
            lightObject.transform.rotation = Quaternion.Euler(42f, -32f, 0f);
        }

        private static void CreateOcean(Transform root)
        {
            GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ocean.name = "Open Ocean";
            ocean.transform.SetParent(root);
            ocean.transform.position = Vector3.zero;
            ocean.transform.localScale = new Vector3(32f, 1f, 32f);
            Renderer renderer = ocean.GetComponent<Renderer>();
            Material water = CreateMaterial(new Color(0.07f, 0.35f, 0.52f), 0.18f, 0.35f);
            Texture2D texture = Resources.Load<Texture2D>(WaterDiffuse);
            if (texture != null)
            {
                water.mainTexture = texture;
                water.mainTextureScale = new Vector2(24f, 24f);
            }
            renderer.material = water;
            ocean.AddComponent<OceanSurfaceAnimator>();
        }

        private static Terrain CreateTerrain(Transform root)
        {
            const int heightResolution = 257;
            const int alphaResolution = 256;
            TerrainData data = new TerrainData
            {
                heightmapResolution = heightResolution,
                alphamapResolution = alphaResolution,
                size = new Vector3(96f, 20f, 96f)
            };

            float[,] heights = new float[heightResolution, heightResolution];
            for (int y = 0; y < heightResolution; y++)
            {
                for (int x = 0; x < heightResolution; x++)
                {
                    float u = x / (float)(heightResolution - 1);
                    float v = y / (float)(heightResolution - 1);
                    float px = u * 2f - 1f;
                    float pz = v * 2f - 1f;
                    float radius = Mathf.Sqrt(px * px + pz * pz * 0.87f);
                    float coast = Mathf.Clamp01(1f - radius);
                    float islandMass = Mathf.Pow(coast, 1.45f) * 0.31f;
                    float ridge = Mathf.PerlinNoise(u * 4.7f + 18f, v * 4.7f + 9f) * 0.045f * coast;
                    float dunes = Mathf.PerlinNoise(u * 14f, v * 11f) * 0.012f * Mathf.Clamp01(1f - islandMass * 3f);
                    heights[y, x] = Mathf.Clamp(0.012f + islandMass + ridge + dunes, 0.012f, 0.40f);
                }
            }
            data.SetHeights(0, 0, heights);

            Texture2D sand = Resources.Load<Texture2D>(BeachDiffuse);
            Texture2D ground = Resources.Load<Texture2D>(GrassDiffuse);
            TerrainLayer sandLayer = new TerrainLayer
            {
                diffuseTexture = sand,
                tileSize = new Vector2(8f, 8f),
                tileOffset = Vector2.zero
            };
            TerrainLayer groundLayer = new TerrainLayer
            {
                diffuseTexture = ground,
                tileSize = new Vector2(10f, 10f),
                tileOffset = Vector2.zero
            };
            data.terrainLayers = new[] { sandLayer, groundLayer };

            float[,,] alpha = new float[alphaResolution, alphaResolution, 2];
            for (int y = 0; y < alphaResolution; y++)
            {
                for (int x = 0; x < alphaResolution; x++)
                {
                    float u = x / (float)(alphaResolution - 1);
                    float v = y / (float)(alphaResolution - 1);
                    float height = heights[Mathf.RoundToInt(v * (heightResolution - 1)), Mathf.RoundToInt(u * (heightResolution - 1))];
                    float organicVariation = Mathf.PerlinNoise(u * 12f + 4f, v * 12f + 7f) * 0.045f;
                    float grassWeight = Mathf.SmoothStep(0f, 1f, (height - 0.08f + organicVariation) / 0.12f);
                    alpha[y, x, 0] = 1f - grassWeight;
                    alpha[y, x, 1] = grassWeight;
                }
            }
            data.SetAlphamaps(0, 0, alpha);

            GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
            terrainObject.name = "Starter Island Terrain";
            terrainObject.transform.SetParent(root);
            terrainObject.transform.position = new Vector3(-48f, -0.02f, -48f);
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            terrain.drawInstanced = true;
            terrain.heightmapPixelError = 4f;
            return terrain;
        }

        private static void CreateLandmarks(Transform root, Terrain terrain)
        {
            Vector3[] palmPositions =
            {
                new Vector3(-18f, 0f, -4f), new Vector3(-14f, 0f, 13f), new Vector3(15f, 0f, 10f),
                new Vector3(21f, 0f, -8f), new Vector3(5f, 0f, 24f), new Vector3(-25f, 0f, 8f),
                new Vector3(10f, 0f, -20f), new Vector3(-4f, 0f, 18f)
            };
            for (int i = 0; i < palmPositions.Length; i++)
            {
                float height = 6.5f + (i % 3) * 1.35f;
                palmPositions[i].y = GroundHeight(terrain, palmPositions[i]) - 0.1f;
                CreatePalm(root, palmPositions[i], height, i * 47f);
            }

            Vector3[] rocks =
            {
                new Vector3(-20f, 0f, -16f), new Vector3(-8f, 0f, 21f), new Vector3(18f, 0f, -17f),
                new Vector3(23f, 0f, 13f), new Vector3(-28f, 0f, 2f), new Vector3(8f, 0f, 28f)
            };
            for (int i = 0; i < rocks.Length; i++)
            {
                rocks[i].y = GroundHeight(terrain, rocks[i]) + 0.25f;
                CreateRock(root, rocks[i], 1.6f + (i % 2) * 0.65f);
            }

            CreateDock(root, terrain);
            CreateCamp(root, terrain);
        }

        private static void CreatePalm(Transform root, Vector3 basePosition, float height, float yaw)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Natural Palm Trunk";
            trunk.transform.SetParent(root);
            trunk.transform.position = basePosition + Vector3.up * (height * 0.5f);
            trunk.transform.rotation = Quaternion.Euler(4f, yaw, -5f);
            trunk.transform.localScale = new Vector3(0.32f, height * 0.5f, 0.32f);
            trunk.GetComponent<Renderer>().material = CreateMaterial(new Color(0.22f, 0.11f, 0.045f), 0.82f, 0f);

            for (int i = 0; i < 7; i++)
            {
                GameObject frond = CreatePalmFrond();
                frond.name = "Palm Frond";
                frond.transform.SetParent(trunk.transform);
                frond.transform.localPosition = new Vector3(0f, height * 0.5f - 0.12f, 0f);
                frond.transform.localRotation = Quaternion.Euler(13f + (i % 2) * 5f, i * (360f / 7f), 0f);
                frond.transform.localScale = Vector3.one * (0.92f + (i % 3) * 0.08f);
            }
        }

        private static GameObject CreatePalmFrond()
        {
            GameObject leaf = new GameObject();
            MeshFilter filter = leaf.AddComponent<MeshFilter>();
            MeshRenderer renderer = leaf.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh { name = "Palm Frond Mesh" };
            mesh.vertices = new[]
            {
                new Vector3(-0.16f, 0f, 0f), new Vector3(0.16f, 0f, 0f),
                new Vector3(-0.35f, -0.10f, 1.65f), new Vector3(0.35f, -0.10f, 1.65f),
                new Vector3(-0.08f, -0.34f, 3.1f), new Vector3(0.08f, -0.34f, 3.1f)
            };
            mesh.triangles = new[] { 0, 2, 1, 1, 2, 3, 2, 4, 3, 3, 4, 5 };
            mesh.uv = new[] { Vector2.zero, Vector2.right, new Vector2(0f, 0.52f), new Vector2(1f, 0.52f), Vector2.up, Vector2.one };
            mesh.RecalculateNormals();
            filter.sharedMesh = mesh;
            renderer.material = CreateMaterial(new Color(0.055f, 0.30f, 0.11f), 0.78f, 0f);
            renderer.shadowCastingMode = ShadowCastingMode.On;
            return leaf;
        }

        private static void CreateRock(Transform root, Vector3 position, float scale)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Weathered Coastal Rock";
            rock.transform.SetParent(root);
            rock.transform.position = position;
            rock.transform.rotation = Quaternion.Euler(11f * scale, 38f * scale, 6f);
            rock.transform.localScale = new Vector3(scale * 1.25f, scale * 0.88f, scale);
            rock.GetComponent<Renderer>().material = CreateMaterial(new Color(0.24f, 0.25f, 0.23f), 0.86f, 0f);
        }

        private static void CreateDock(Transform root, Terrain terrain)
        {
            Vector3 start = new Vector3(-31f, 0f, -13f);
            start.y = GroundHeight(terrain, start) + 0.25f;
            Material wood = CreateMaterial(new Color(0.24f, 0.10f, 0.035f), 0.72f, 0f);
            for (int i = 0; i < 9; i++)
            {
                GameObject plank = GameObject.CreatePrimitive(PrimitiveType.Cube);
                plank.name = "Dock Plank";
                plank.transform.SetParent(root);
                plank.transform.position = start + new Vector3(-i * 1.15f, 0f, 0.15f * i);
                plank.transform.rotation = Quaternion.Euler(0f, -16f, 0f);
                plank.transform.localScale = new Vector3(1.05f, 0.14f, 2.6f);
                plank.GetComponent<Renderer>().material = wood;
            }
        }

        private static void CreateCamp(Transform root, Terrain terrain)
        {
            Vector3 camp = new Vector3(0f, 0f, 10f);
            camp.y = GroundHeight(terrain, camp);
            Material canvas = CreateMaterial(new Color(0.42f, 0.09f, 0.07f), 0.82f, 0f);
            GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            canopy.name = "Raider Camp Canopy";
            canopy.transform.SetParent(root);
            canopy.transform.position = camp + Vector3.up * 1.5f;
            canopy.transform.localScale = new Vector3(2.7f, 0.18f, 2.7f);
            canopy.GetComponent<Renderer>().material = canvas;

            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                post.name = "Camp Support";
                post.transform.SetParent(root);
                post.transform.position = camp + new Vector3(Mathf.Cos(angle) * 2.2f, 0.75f, Mathf.Sin(angle) * 2.2f);
                post.transform.localScale = new Vector3(0.12f, 0.75f, 0.12f);
                post.GetComponent<Renderer>().material = CreateMaterial(new Color(0.20f, 0.08f, 0.025f), 0.7f, 0f);
            }
        }

        private static float GroundHeight(Terrain terrain, Vector3 position)
        {
            return terrain.SampleHeight(position) + terrain.transform.position.y;
        }

        private static Material CreateMaterial(Color color, float smoothness, float metallic)
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
            return material;
        }
    }
}

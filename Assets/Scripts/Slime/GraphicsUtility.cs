using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class GraphicsUtility
{
    public static void dispatch(ref ComputeShader shader, int kernelID, int groupsX, int groupsY)
    {
        Vector3Int threadGroupSizes = getThreadGroupSizes(shader, kernelID);
        int numGroupsX = Mathf.CeilToInt(groupsX / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(groupsY / (float)threadGroupSizes.y);

        shader.Dispatch(kernelID, numGroupsX, numGroupsY, 1);
    }

    public static Vector3Int getThreadGroupSizes(ComputeShader compute, int kernelIndex = 0)
    {
        uint x, y, z;
        compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }

    public static void createStructuredBuffer<T>(ref ComputeBuffer buffer, T[] bufferData)
    {
        int stride = Marshal.SizeOf(typeof(T));
        bool createNewBuffer = buffer == null || !buffer.IsValid() || buffer.count != bufferData.Length || buffer.stride != stride;
        if (createNewBuffer)
        {
            Release(buffer);
            buffer = new ComputeBuffer(bufferData.Length, stride);
        }

        buffer.SetData(bufferData);
    }

    public static void setupShaderBuffer<T>(ref ComputeBuffer computeBuffer, ComputeShader computeShader, T[] bufferData, int kernel, string bufferName)
    {
        createStructuredBuffer<T>(ref computeBuffer, bufferData);
        computeShader.SetBuffer(kernel, bufferName, computeBuffer);
    }

    public static void createRenderTexture(ref RenderTexture texture, int width, int height, FilterMode filterMode, GraphicsFormat format)
    {
        if (texture == null || !texture.IsCreated() || texture.width != width || texture.height != height || texture.graphicsFormat != format)
        {
            if (texture != null)
            {
                texture.Release();
            }
            texture = new RenderTexture(width, height, 0);
            texture.graphicsFormat = format;
            texture.enableRandomWrite = true;

            texture.autoGenerateMips = false;
            texture.Create();
        }
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = filterMode;
    }

    public static void Release(params ComputeBuffer[] buffers)
    {
        foreach(ComputeBuffer buffer in buffers)
        {
            if (buffer != null)
            {
                buffer.Release();
            }
        }
    }
}

#include "ModelRenderer.h"
#include "Scene.h"
#include<iostream>

namespace Luminal
{
    void ModelRenderer::OnCreate()
    {
        std::cout << "poggers" << std::endl;
    }

    void ModelRenderer::SetShaderVariables()
    {
        Scene::Program.Uniform3f("aMaterial.Albedo", Material.Albedo);
        Scene::Program.Uniform3f("aMaterial.Specular", Material.Specular);

        Scene::Program.Uniform1i("aMaterial.UseAlbedoMap", Material.HasAlbedo());
        Scene::Program.Uniform1i("aMaterial.UseSpecularMap", Material.HasSpecular());

        Scene::Program.Uniform1f("aMaterial.Shininess", Material.Shininess);
    }

    void ModelRenderer::OnRender()
    {
        Scene::Program.Use();

        SetShaderVariables();

        Scene::Program.UniformMatrix4("Model", parent->Model());

        Model->Draw(Scene::Program);
    }

    void ModelRenderer::LoadModel(const char* path)
    {
        Model = std::make_shared<Luminal::Model>();
        Model->Load(path);
    }
}
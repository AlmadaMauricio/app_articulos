﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using negocio;
using System.Reflection;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "SELECT Codigo, Nombre, A.Descripcion AS ArticuloDescripcion, ImagenUrl, C.Id AS CategoriaId, C.Descripcion AS CategoriaDescripcion, M.Id AS MarcaId, M.Descripcion AS MarcaDescripcion, Precio, A.Id FROM ARTICULOS A INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id INNER JOIN MARCAS M ON A.IdMarca = M.Id";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)lector["Id"];
                    aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["ArticuloDescripcion"];


                    if (!(lector["ImagenUrl"] is DBNull))
                    {
                        aux.ImagenUrl = (string)lector["ImagenUrl"];
                    }



                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)lector["MarcaId"];
                    aux.Marca.Descripcion = (string)lector["MarcaDescripcion"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)lector["CategoriaId"];
                    aux.Categoria.Descripcion = (string)lector["CategoriaDescripcion"];
                    aux.Precio = (float)Convert.ToDecimal(lector["Precio"]);

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio)VALUES (@Codigo, @Nombre, @Descripcion, @IdMarca, @IdCategoria, @ImagenUrl, @Precio)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("ImagenUrl", nuevo.ImagenUrl);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void modificar(Articulo articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Update ARTICULOS set Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion, IdMarca = @IdMarca, IdCategoria = @IdCategoria, ImagenUrl = @ImagenUrl, Precio = @Precio from ARTICULOS Where Id = @Id");
                datos.setearParametro("@Codigo", articulo.Codigo);
                datos.setearParametro("@Nombre", articulo.Nombre);
                datos.setearParametro("@Descripcion", articulo.Descripcion);
                datos.setearParametro("@IdMarca", articulo.Marca.Id);
                datos.setearParametro("@IdCategoria", articulo.Categoria.Id);
                datos.setearParametro("@ImagenUrl", articulo.ImagenUrl);
                datos.setearParametro("@Precio", articulo.Precio);
                datos.setearParametro("@Id", articulo.Id);

                datos.ejecutarAccion();
            }

            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("Delete from articulos where id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                /*string consulta = "SELECT Codigo, Nombre, A.Descripcion AS ArticuloDescripcion, ImagenUrl, C.Id AS CategoriaId, C.Descripcion AS CategoriaDescripcion, M.Id AS MarcaId, M.Descripcion AS MarcaDescripcion, Precio, A.Id FROM ARTICULOS A INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id INNER JOIN MARCAS M ON A.IdMarca = M.Id And ";*/
                string consulta = "SELECT A.Codigo, A.Nombre, A.Descripcion AS ArticuloDescripcion, ImagenUrl, C.Id AS CategoriaId, C.Descripcion AS CategoriaDescripcion, M.Id AS MarcaId, M.Descripcion AS MarcaDescripcion, Precio, A.Id FROM ARTICULOS A INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id INNER JOIN MARCAS M ON A.IdMarca = M.Id";

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    if (campo == "Código")
                    {
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += " Where A.Codigo like '" + filtro + "%'";
                                break;

                            case "Termina con":
                                consulta += " Where A.Codigo like '%" + filtro + "'";
                                break;

                            default:
                                consulta += " Where A.Codigo like '%" + filtro + "%'";
                                break;
                        }
                    }
                    else if (campo == "Nombre")
                    {
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += " Where A.Nombre like '" + filtro + "%'";
                                break;

                            case "Termina con":
                                consulta += " Where A.Nombre like '%" + filtro + "'";
                                break;
                            default:
                                consulta += " Where A.Nombre like '%" + filtro + "%'";
                                break;
                        }
                    }
                    else if (campo == "Precio")
                    {
                        switch (criterio)
                        {
                            case "Mayor a":
                                consulta += " Where A.Precio > " + filtro;
                                break;

                            case "Menor a":
                                consulta += " Where A.Precio < " + filtro;
                                break;
                            default:
                                consulta += " Where A.Precio = " + filtro;
                                break;
                        }
                    }
                    else
                    {
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += " Where A.Descripcion like '" + filtro + "%'";
                                break;

                            case "Termina con":
                                consulta += " Where A.Descripcion like '%" + filtro + "'";
                                break;
                            default:
                                consulta += " Where A.Descripcion like '%" + filtro + "%'";
                                break;
                        }
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["ArticuloDescripcion"];


                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                    {
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    }



                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["MarcaId"];
                    aux.Marca.Descripcion = (string)datos.Lector["MarcaDescripcion"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["CategoriaId"];
                    aux.Categoria.Descripcion = (string)datos.Lector["CategoriaDescripcion"];
                    aux.Precio = (float)Convert.ToDecimal(datos.Lector["Precio"]);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
    }
}

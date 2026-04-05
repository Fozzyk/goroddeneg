// src/stores/projects.js
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { projectsApi, categoriesApi } from '@/services/api'

export const useProjectsStore = defineStore('projects', () => {
  const featured   = ref([])
  const categories = ref([])
  const loading    = ref(false)

  async function fetchFeatured(pageSize = 4) {
    loading.value = true
    try {
      const { data } = await projectsApi.getAll({ pageSize, page: 1 })
      featured.value = data.items
    } catch (e) {
      console.error('fetchFeatured failed', e)
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories() {
    if (categories.value.length) return
    try {
      const { data } = await categoriesApi.getAll()
      categories.value = data
    } catch (e) {
      console.error('fetchCategories failed', e)
    }
  }

  return { featured, categories, loading, fetchFeatured, fetchCategories }
})

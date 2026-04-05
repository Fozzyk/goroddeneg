<template>
  <div class="page">
    <div class="container">
      <!-- Header -->
      <div class="projects-header">
        <h1>Проекты</h1>
        <p class="text-muted mt-2">Поддержите идеи, которые меняют Казахстан</p>
      </div>

      <!-- Search + filters -->
      <div class="filters-bar">
        <div class="search-wrap">
          <span class="search-icon">🔍</span>
          <input
            v-model="search"
            class="search-input"
            placeholder="Поиск по названию..."
            @input="debouncedFetch"
          />
          <button v-if="search" class="search-clear" @click="search=''; fetchProjects()">✕</button>
        </div>
        <div class="sort-wrap">
          <label class="form-label" style="margin:0">Сортировка:</label>
          <select v-model="sort" class="form-control" style="width:auto" @change="fetchProjects">
            <option value="default">По умолчанию</option>
            <option value="funded">По сборам</option>
            <option value="newest">Новые</option>
            <option value="ending">Скоро завершаются</option>
          </select>
        </div>
      </div>

      <!-- Categories -->
      <div class="cats-row">
        <button
          class="cat-chip"
          :class="{ active: !activeCat }"
          @click="activeCat = null; fetchProjects()"
        >
          Все проекты
        </button>
        <button
          v-for="c in categories"
          :key="c.id"
          class="cat-chip"
          :class="{ active: activeCat === c.id }"
          @click="activeCat = c.id; fetchProjects()"
        >
          {{ c.icon }} {{ c.name }}
        </button>
      </div>

      <!-- Promo Banner -->
      <div class="promo-banner">
        <div class="promo-content">
          <div class="promo-emoji">💡</div>
          <div>
            <h3 class="promo-title">У тебя есть крутая идея?</h3>
            <p class="promo-sub">Перейди от мечтаний к действию! Запусти проект за 15 минут.</p>
          </div>
        </div>
        <router-link to="/create" class="btn btn-primary">Создать проект →</router-link>
      </div>

      <!-- Results info -->
      <div class="results-info" v-if="!loading">
        <span>{{ total }} проектов найдено</span>
        <span v-if="activeCat || search" class="clear-all" @click="clearAll">✕ Сбросить фильтры</span>
      </div>

      <!-- Grid -->
      <div v-if="loading" class="proj-grid">
        <div v-for="i in 8" :key="i" class="skeleton-card" />
      </div>

      <div v-else-if="projects.length" class="proj-grid">
        <ProjectCard v-for="p in projects" :key="p.id" :project="p" />
      </div>

      <div v-else class="empty-state">
        <div class="empty-icon">🔍</div>
        <h3>Ничего не найдено</h3>
        <p>Попробуйте изменить фильтры или поисковый запрос</p>
        <button class="btn btn-outline mt-4" @click="clearAll">Сбросить всё</button>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="pagination">
        <button class="btn btn-ghost btn-sm" :disabled="page === 1" @click="page--; fetchProjects()">← Назад</button>
        <div class="page-nums">
          <button
            v-for="p in pageNumbers"
            :key="p"
            class="page-num"
            :class="{ active: p === page }"
            @click="page = p; fetchProjects()"
          >{{ p }}</button>
        </div>
        <button class="btn btn-ghost btn-sm" :disabled="page === totalPages" @click="page++; fetchProjects()">Вперёд →</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import ProjectCard from '@/components/project/ProjectCard.vue'
import { projectsApi, categoriesApi } from '@/services/api'

const projects   = ref([])
const categories = ref([])
const loading    = ref(true)
const search     = ref('')
const activeCat  = ref(null)
const sort       = ref('default')
const page       = ref(1)
const total      = ref(0)
const pageSize   = 12
const totalPages = computed(() => Math.ceil(total.value / pageSize))
const pageNumbers = computed(() => {
  const pages = []; const s = Math.max(1, page.value-2), e = Math.min(totalPages.value, page.value+2)
  for (let i = s; i <= e; i++) pages.push(i)
  return pages
})

let debTimer = null
function debouncedFetch() {
  clearTimeout(debTimer)
  debTimer = setTimeout(() => { page.value = 1; fetchProjects() }, 400)
}

async function fetchProjects() {
  loading.value = true
  try {
    const { data } = await projectsApi.getAll({
      categoryId: activeCat.value || undefined,
      search:     search.value || undefined,
      page:       page.value,
      pageSize
    })
    projects.value = data.items
    total.value    = data.totalCount
  } catch {}
  loading.value = false
}

function clearAll() { search.value = ''; activeCat.value = null; page.value = 1; fetchProjects() }

onMounted(async () => {
  const [, cats] = await Promise.allSettled([fetchProjects(), categoriesApi.getAll()])
  if (cats.status === 'fulfilled') categories.value = cats.value.data
})
</script>

<style scoped>
.projects-header { margin-bottom: 32px; }
.projects-header h1 { margin-bottom: 4px; }

.filters-bar { display: flex; gap: 16px; align-items: center; margin-bottom: 20px; flex-wrap: wrap; }
.search-wrap {
  flex: 1; min-width: 220px; position: relative;
  display: flex; align-items: center;
  background: var(--warm-white); border: 1.5px solid var(--border);
  border-radius: var(--r-md); padding: 0 14px;
  transition: border-color .2s;
}
.search-wrap:focus-within { border-color: var(--gold); box-shadow: 0 0 0 3px rgba(166,124,0,.1); }
.search-icon  { font-size: 16px; color: var(--text-muted); flex-shrink: 0; }
.search-input { flex: 1; border: none; outline: none; padding: 11px 10px; font-size: 14px; font-family: 'DM Sans', sans-serif; background: transparent; color: var(--text-primary); }
.search-input::placeholder { color: var(--text-light); }
.search-clear { background: none; border: none; color: var(--text-muted); cursor: pointer; font-size: 14px; padding: 4px; border-radius: 50%; }
.search-clear:hover { color: var(--danger); background: var(--danger-bg); }
.sort-wrap { display: flex; align-items: center; gap: 10px; flex-shrink: 0; }

.cats-row { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 28px; }
.cat-chip {
  padding: 8px 18px;
  border-radius: var(--r-full);
  border: 2px solid var(--border);
  background: var(--bg);
  font-size: 13px;
  font-weight: 500;
  color: var(--text-mid);
  cursor: pointer;
  transition: all .18s;
  white-space: nowrap;
  font-family: 'Inter', sans-serif;
  box-shadow: var(--shadow-xs);
}
.cat-chip:hover {
  border-color: var(--blue);
  color: var(--blue);
  background: var(--blue-light);
}
.cat-chip.active {
  background: var(--blue);
  color: #fff;
  border-color: var(--blue);
  box-shadow: var(--shadow-blue);
  font-weight: 600;
}

.promo-banner {
  background: linear-gradient(135deg, var(--gold-subtle), var(--gold-pale));
  border: 1px solid rgba(166,124,0,.2); border-radius: var(--r-lg);
  padding: 24px 32px; display: flex; justify-content: space-between;
  align-items: center; gap: 24px; margin-bottom: 32px;
}
.promo-content { display: flex; align-items: center; gap: 20px; }
.promo-emoji   { font-size: 40px; }
.promo-title   { font-size: 1.1rem; font-weight: 700; color: var(--text-primary); margin-bottom: 4px; }
.promo-sub     { font-size: 13px; color: var(--text-muted); }

.results-info {
  display: flex; justify-content: space-between; align-items: center;
  font-size: 13px; color: var(--text-muted); margin-bottom: 20px;
}
.clear-all { color: var(--gold); cursor: pointer; font-weight: 600; }
.clear-all:hover { text-decoration: underline; }

.proj-grid { display: grid; grid-template-columns: repeat(4,1fr); gap: 22px; margin-bottom: 48px; }
.skeleton-card { height: 360px; border-radius: var(--r-lg); background: linear-gradient(90deg, var(--sand) 25%, var(--sand-dark) 50%, var(--sand) 75%); background-size: 200% 100%; animation: shimmer 1.4s infinite; }
@keyframes shimmer { 0%{background-position:200% 0} 100%{background-position:-200% 0} }

.empty-state { text-align: center; padding: 80px 24px; }
.empty-icon  { font-size: 56px; margin-bottom: 16px; }
.empty-state h3 { margin-bottom: 8px; }

.pagination { display: flex; justify-content: center; align-items: center; gap: 8px; }
.page-nums  { display: flex; gap: 6px; }
.page-num {
  width: 36px; height: 36px; border-radius: var(--r-sm);
  border: 1.5px solid var(--border); background: var(--warm-white);
  font-size: 13px; font-weight: 600; cursor: pointer; color: var(--text-muted);
  display: flex; align-items: center; justify-content: center; font-family: 'DM Sans', sans-serif;
}
.page-num.active { background: var(--gold); color: #fff; border-color: var(--gold); }
.page-num:not(.active):hover { border-color: var(--gold); color: var(--gold); }

@media (max-width: 900px) { .proj-grid { grid-template-columns: repeat(2,1fr); } }
@media (max-width: 540px) { .proj-grid { grid-template-columns: 1fr; } .promo-banner { flex-direction: column; text-align: center; } }
</style>

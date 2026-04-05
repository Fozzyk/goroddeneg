<template>
  <div v-if="totalPages > 1" class="pagination">
    <button
      class="btn btn-ghost btn-sm"
      :disabled="page <= 1"
      @click="$emit('update:page', page - 1)"
    >← Назад</button>

    <div class="page-nums">
      <button
        v-if="showFirst"
        class="page-num"
        @click="$emit('update:page', 1)"
      >1</button>
      <span v-if="showFirstEllipsis" class="page-ellipsis">…</span>

      <button
        v-for="p in visiblePages"
        :key="p"
        class="page-num"
        :class="{ active: p === page }"
        @click="$emit('update:page', p)"
      >{{ p }}</button>

      <span v-if="showLastEllipsis" class="page-ellipsis">…</span>
      <button
        v-if="showLast"
        class="page-num"
        @click="$emit('update:page', totalPages)"
      >{{ totalPages }}</button>
    </div>

    <button
      class="btn btn-ghost btn-sm"
      :disabled="page >= totalPages"
      @click="$emit('update:page', page + 1)"
    >Вперёд →</button>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  page:       { type: Number, required: true },
  totalPages: { type: Number, required: true },
  window:     { type: Number, default: 2 },
})
defineEmits(['update:page'])

const visiblePages = computed(() => {
  const start = Math.max(1, props.page - props.window)
  const end   = Math.min(props.totalPages, props.page + props.window)
  const pages = []
  for (let i = start; i <= end; i++) pages.push(i)
  return pages
})
const showFirst         = computed(() => visiblePages.value[0] > 1)
const showLast          = computed(() => visiblePages.value.at(-1) < props.totalPages)
const showFirstEllipsis = computed(() => visiblePages.value[0] > 2)
const showLastEllipsis  = computed(() => visiblePages.value.at(-1) < props.totalPages - 1)
</script>

<style scoped>
.pagination {
  display: flex; justify-content: center; align-items: center;
  gap: 8px; margin-top: 40px; flex-wrap: wrap;
}
.page-nums    { display: flex; gap: 6px; }
.page-num {
  width: 38px; height: 38px; border-radius: var(--r-sm);
  border: 1.5px solid var(--border); background: var(--warm-white);
  font-size: 14px; font-weight: 600; cursor: pointer;
  color: var(--text-muted); display: flex; align-items: center;
  justify-content: center; transition: all .18s; font-family: 'DM Sans', sans-serif;
}
.page-num.active { background: var(--gold); color: #fff; border-color: var(--gold); }
.page-num:not(.active):hover { border-color: var(--gold); color: var(--gold); background: var(--gold-subtle); }
.page-ellipsis { display: flex; align-items: center; color: var(--text-muted); font-size: 16px; padding: 0 4px; }
</style>

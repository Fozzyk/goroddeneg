<template>
  <div class="skeleton-card-wrap card">
    <div class="sk-cover" />
    <div class="sk-body">
      <div class="sk-line sk-cat" />
      <div class="sk-line sk-title" />
      <div class="sk-line sk-title sk-title-short" />
      <div class="sk-line sk-desc" />
      <div class="sk-line sk-desc sk-desc-short" />
      <div class="sk-progress" />
      <div class="sk-footer">
        <div class="sk-line sk-stat" />
        <div class="sk-line sk-stat" />
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes shimmer {
  0%   { background-position: 200% 0 }
  100% { background-position: -200% 0 }
}
.skeleton-card-wrap { overflow: hidden; }

.sk-cover {
  height: 190px;
  background: linear-gradient(90deg, var(--sand) 25%, var(--sand-dark) 50%, var(--sand) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

.sk-body { padding: 18px 20px 20px; }

.sk-line {
  border-radius: 4px;
  background: linear-gradient(90deg, var(--sand) 25%, var(--sand-dark) 50%, var(--sand) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  margin-bottom: 10px;
  height: 12px;
}

.sk-cat         { width: 60px;  height: 10px; margin-bottom: 12px; }
.sk-title       { width: 100%;  height: 16px; }
.sk-title-short { width: 70%;   }
.sk-desc        { width: 100%;  height: 11px; }
.sk-desc-short  { width: 85%;   }

.sk-progress {
  height: 6px; border-radius: 3px; margin: 14px 0;
  background: linear-gradient(90deg, var(--sand) 25%, var(--sand-dark) 50%, var(--sand) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

.sk-footer { display: flex; justify-content: space-between; margin-top: 14px; }
.sk-stat   { width: 30%; height: 11px; margin-bottom: 0; }
</style>
